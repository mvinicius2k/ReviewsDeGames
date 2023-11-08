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
    [ApiController, Route(Route)]
    public class UserVotesController : ControllerBase
    {
        private readonly IUserVoteRepository _votes;
        private readonly ILogger<UserVotesController> _logger;
        private readonly IMapper _mapper;
        private readonly IDescribesService _describesService;
        private readonly IValidator<UserVoteRequestDto> _validator;

        public const int MinScore = 0;
        public const int MaxScore = 10;

        public const string Route = "UserVotes";
        public const string ActionGet = "get";
        public const string ActionVote = "vote";
        public const string ActionUnvote = $"unvote/{{{UnvoteRoutePostId}}}";

        public const string UnvoteRoutePostId = "userId";

        public UserVotesController(IUserVoteRepository votes, ILogger<UserVotesController> logger, IMapper mapper, IDescribesService describesService, IValidator<UserVoteRequestDto> validator)
        {
            _votes = votes;
            _logger = logger;
            _mapper = mapper;
            _describesService = describesService;
            _validator = validator;
        }

        /// <summary>
        /// Obtém uma consulta com base na query OData passada pela url
        /// </summary>
        /// <response code="200">Sucesso</response>
        [Route(ActionGet), HttpGet, EnableQuery]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IQueryable<UserVote> Get()
            => _votes.GetQuery();

        /// <summary>
        /// Adiciona ou altera o voto do usuário logado no post passado
        /// </summary>
        /// <response code="201">Sucesso se o voto foi adicionado</response>
        /// <response code="200">Sucesso se o voto foi alterado</response>
        /// <response code="422">Algum dado inválido no modelo</response>
        [HttpPut, Route(ActionVote), Authorize]
        public async Task<IActionResult> Vote(UserVoteRequestDto dto)
        {
            var loggedUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {

                validation.AddToModelState(ModelState);
                return UnprocessableEntity(ModelState);
            }

            var vote = _mapper.Map<UserVote>(dto);
            vote.Value = Math.Clamp(vote.Value, MinScore, MaxScore); //Obtendo um valor entre 0 e 10
            vote.UserId = loggedUserId;

            //Retornando resposta com base na existência ou não de um voto já registrado
            var exists = (await _votes.GetById(vote.GetId())) != null;
            if (exists)
            {
                await _votes.Update(vote.GetId(), vote);
                return Ok(vote);
            }
            else
            {
                await _votes.Create(vote);
                return CreatedAtAction(ActionVote, vote);
            }
        }

        /// <summary>
        /// Remove o voto do post
        /// </summary>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Se o post não existe</response>
        [HttpDelete, Route(ActionUnvote), Authorize]
        public async Task<IActionResult> Unvote([FromRoute(Name = UnvoteRoutePostId)] int postId)
        {
            var loggedUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await _votes.Delete((loggedUserId, postId));
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(_describesService.EntityNotFound(nameof(UserVote), (loggedUserId, postId)));
            }
        }
    }
}
