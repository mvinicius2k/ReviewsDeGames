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

        [Route(ActionGet), HttpGet, EnableQuery]
        public IQueryable<Post> Get()
            => _posts.GetQuery();

        [HttpPost, Route(ActionCreate)]
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

        [HttpPatch, Route(ActionEdit)]
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
            post.LastEdit = DateTime.UtcNow;

            await _posts.Update(id, post);

            return CreatedAtAction(ActionCreate, post);
        }

        [HttpDelete, Route(ActionDelete)]
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
