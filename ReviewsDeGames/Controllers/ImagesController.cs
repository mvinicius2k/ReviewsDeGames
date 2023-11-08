using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using ReviewsDeGames.Helpers;
using ReviewsDeGames.Models;
using ReviewsDeGames.Repository;
using ReviewsDeGames.Services;
using System.Security.Claims;

namespace ReviewsDeGames.Controllers
{
    [Route(Route), ApiController]
    public class ImagesController : ControllerBase
    {
        #region Constants
        public const string Route = "Images";

        public const string ActionGet = "get";
        public const string ActionAdd = "add";
        public const string ActionDelete = $"delete/{{{DeleteIdRoute}}}";

        public const string DeleteIdRoute = "id";
        public const string AddFilesForm = "files";
        #endregion

        private readonly IHostImageService _hostImage;
        private readonly IImagesRepository _images;
        private readonly IDescribesService _describes;
        private readonly IValidator<ImageRequestDto> _validator;
        private readonly ILogger<ImagesController> _logger;
        private readonly IMapper _mapper;

        public ImagesController(IHostImageService hostImage, IImagesRepository images, IDescribesService describes, IValidator<ImageRequestDto> validator, ILogger<ImagesController> logger, IMapper mapper)
        {
            _hostImage = hostImage;
            _images = images;
            _describes = describes;
            _validator = validator;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtém uma lista de <see cref="Image"/> de acordo com a query OData passada.
        /// </summary>
        /// <response code="200">Retorna uma consulta do OData</response>
        [EnableQuery, Route(ActionGet), HttpGet]
        public IQueryable<Image> Get() 
            => _images.GetQuery();


        /// <summary>
        /// Envia uma coleção de imagens para o servidor
        /// </summary>
        /// <response code="207">Uma lista de status para cada imagem enviada</response>
        /// <response code="204">Foi enviado uma lista vazia. Nada será feito</response>
        [HttpPost, Route(ActionAdd)]
        public async Task<IActionResult> Add([FromForm(Name = AddFilesForm)]IFormFileCollection files)
        {
            //Nada a processar
            if (files.Count == 0)
                return NoContent();
            var loggedUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            //Lista de resposta multistatus, será retornada ao client com o resultado do processamente de cada imagem
            var multiStatus = new List<MultiStatusResponse<ImageResponseDto>>(files.Count);


            //Processando imagens um a um
            foreach (var file in files)
            {
                var dto = new ImageRequestDto
                {
                    FormFile = file,
                };

                //Validando
                var validation = await _validator.ValidateAsync(dto);
                if (!validation.IsValid)
                {
                    
                    multiStatus.Add(new MultiStatusResponse<ImageResponseDto>
                    {
                        StatusCode = StatusCodes.Status422UnprocessableEntity,
                        Message = $"{file.FileName}: {string.Join("; ", validation.Errors.Select(err => err.ErrorMessage))}",
                        Result = null
                    }) ;
                    continue; //Pulando para proxima imagem caso a atual seja inválida
                }

                //Criando modelo para registrar no banco de dados
                var newImage = _mapper.Map<Image>(dto);
                newImage.OwnerId = loggedUserId;
                newImage.FileName = _hostImage.UniqueName(newImage.FileName); //Obtendo nome único

                try
                {
                    //Transformando imagem em bytes e enviando para o serviço de armazenamento de imagens
                    var bytes = new byte[dto.FormFile.Length];
                    using (var stream = dto.FormFile.OpenReadStream())
                        stream.Read(bytes, 0, bytes.Length);
                    
                    var url = await _hostImage.Write(bytes, newImage.FileName, newImage.OwnerId, DateOnly.FromDateTime(newImage.UploadedAt));

                    //Registrando no BD
                    await _images.Create(newImage);

                    var response = _mapper.Map<ImageResponseDto>(newImage);
                    response.Url = url;
                    multiStatus.Add(new MultiStatusResponse<ImageResponseDto>
                    {
                        StatusCode = StatusCodes.Status201Created,
                        Message = null,
                        Result = response
                    });
                }
                catch (Exception e)
                {
                    
                    _logger.LogError(e, e.Message);
                    multiStatus.Add(new MultiStatusResponse<ImageResponseDto>
                    {
                        StatusCode = StatusCodes.Status409Conflict,
                        Message = $"{file.FileName}: {e.Message}",
                        Result = null
                    });
                }


            }

            return StatusCode(StatusCodes.Status207MultiStatus, multiStatus);


        }

        /// <summary>
        /// Deleta a imagem do servidor e escreve <see langword="null"/> em todas as entidades que a referenciam
        /// </summary>
        /// <response code="200">Se a operação foi um sucesso</response>
        /// <response code="401">Em erros de autenticação e permissão</response>
        /// <response code="409">Algum erro ocorra</response>
        [HttpDelete, Route(ActionDelete), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status401Unauthorized), ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete([FromRoute(Name = DeleteIdRoute)] int id)
        {
            
            var loggedUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!_images.GetQuery().Any(i => i.OwnerId == loggedUserId))
                return Unauthorized();

            var image = await _images.GetById(id);
            if(image == null)
            {
                ModelState.AddModelError(nameof(_describes.KeyNotFound), _describes.KeyNotFound(id));
                return NotFound(ModelState);
            }

            try
            {
                //Deletando arquivo de imagem
                _hostImage.Delete(image.FileName, image.OwnerId, DateOnly.FromDateTime(image.UploadedAt));
                //Deletando entrada no BD
                await _images.Delete(id);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                ModelState.AddModelError(e.InnerException?.GetType().Name ?? typeof(Exception).Name, e.Message);
                return Conflict(ModelState);
            }
        }

    }
}
