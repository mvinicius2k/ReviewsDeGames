using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using ReviewsDeGames.Helpers;
using ReviewsDeGames.Models;
using ReviewsDeGames.Repository;
using ReviewsDeGames.Services;
using System.Security.Claims;

namespace ReviewsDeGames.Controllers
{
    [ApiController, Route(Route)]
    public class PostsController : ControllerBase
    {
        #region Constants
        public const string Route = "Posts";
        public const string ActionGet = "get";
        public const string ActionCreate = "create";
        public const string ActionEdit = $"edit/{{{EditRouteId}}}";
        public const string ActionDelete = $"delete/{{{DeleteRouteId}}}";

        public const string EditRouteId = "id";
        public const string DeleteRouteId = "id";
        #endregion

        private readonly IPostRepository _posts;
        private readonly ILogger<PostsController> _logger;
        private readonly IValidator<PostRequestDto> _validator;
        private readonly IMapper _mapper;
        private readonly IDescribesService _describesService;

        public PostsController(IPostRepository posts, ILogger<PostsController> logger, IValidator<PostRequestDto> validator, IMapper mapper, IDescribesService describesService)
        {
            _posts = posts;
            _logger = logger;
            _validator = validator;
            _mapper = mapper;
            _describesService = describesService;
        }

        /// <summary>
        /// Obtém uma consulta de <see cref="Post"/> para a query OData passada pela url
        /// </summary>
        /// <response code="200">Se a operação foi um sucesso</response>
        [Route(ActionGet), HttpGet, EnableQuery]
        public IQueryable<Post> Get()
            => _posts.GetQuery();

        /// <summary>
        /// Cria um novo post
        /// </summary>
        /// <response code="422">Algum erro de validação do body</response>
        /// <response code="201">Sucesso</response>
        [HttpPost, Route(ActionCreate)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity), ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(PostRequestDto postDto)
        {
            var validation = await _validator.ValidateAsync(postDto);
            if (!validation.IsValid)
            {
                validation.AddToModelState(ModelState);
                return UnprocessableEntity(ModelState);
            }
            string? userIdRequest = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = _mapper.Map<Post>(postDto);
            post.OwnerId = userIdRequest;
            post.PublicationDate = DateTime.UtcNow;
            await _posts.Create(post);

            return CreatedAtAction(ActionCreate, post);
        }

        /// <summary>
        /// Edita um post
        /// </summary>
        /// <response code="404">Caso o post a editar não exista</response>
        /// <response code="401">Caso não esteja autenticado corretamente ou esteja editando um post que não é do usuário</response>
        /// <response code="422">Caso o objeto body seja inválido</response>
        /// <response code="200">Sucesso</response>
        [HttpPatch, Route(ActionEdit)]
        [ProducesResponseType(StatusCodes.Status404NotFound), ProducesResponseType(StatusCodes.Status401Unauthorized), ProducesResponseType(StatusCodes.Status422UnprocessableEntity), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Edit([FromRoute(Name = EditRouteId)] int id, PostRequestDto postDto)
        {
            
            var postToEdit = await _posts.GetById(id);
            if (postToEdit == null)
                return NotFound(_describesService.KeyNotFound(id));

            var userIdRequest = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdRequest != postToEdit.OwnerId)
                return Unauthorized();

            var validation = await _validator.ValidateAsync(postDto);
            if (!validation.IsValid)
            {
                validation.AddToModelState(ModelState);
                return UnprocessableEntity(ModelState);
            }

            var post = _mapper.Map<Post>(postDto);
            post.OwnerId = userIdRequest;
            post.LastEdit = DateTime.UtcNow;

            await _posts.Update(id, post);

            return Ok(post);
        }

        /// <summary>
        /// Deleta um post, adiciona null em todas as entidades que o referencia e deleta também em cascata os votos e outras entidades dependentes
        /// </summary>
        /// <response code="404">Caso o post a deletar não exista</response>
        /// <response code="401">Caso não esteja autenticado corretamente ou esteja deletando um post que não é do usuário</response>
        /// <response code="200">Sucesso</response>
        [HttpDelete, Route(ActionDelete)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized), ProducesResponseType(StatusCodes.Status404NotFound), ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromRoute(Name = DeleteRouteId)] int id)
        {
            var postToEdit = await _posts.GetById(id);
            if (postToEdit == null)
                return NotFound(_describesService.KeyNotFound(id));

            var userIdRequest = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdRequest != postToEdit.OwnerId)
                return Unauthorized();

            await _posts.Delete(id);
            return Ok();
        }
    }
}
