using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using ReviewsDeGames.Helpers;
using ReviewsDeGames.Models;
using ReviewsDeGames.Repository;

namespace ReviewsDeGames.Controllers
{
    [ApiController, Route(Route)]
    public class UsersController : ControllerBase
    {
        public const string Route = "Users";

        public const string ActionRegister = "register";
        public const string ActionPatchAvatar = "patchAvatar";
        public const string ActionLogin = "login";
        public const string ActionPatchPassword = "patchPassword";
        public const string ActionPatchInfos = "patchInfos";
        public const string ActionGet = "get";

        public const string PatchAvatarIdHeader = "Id";
        public const string PatchAvatarUrlHeader = "Avatar-Url";
        public const string PatchPasswordUserIdHeader = "User-Id";
        public const string PatchPasswordCurrentPasswordHeader = "Current-Pass";
        public const string PatchPasswordNewPasswordHeader = "New-Pass";

        private readonly IValidator<UserRegisterDto> _validator;
        private readonly ILogger<UsersController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserRepository _users;
        private readonly UserManager<User> _userManager;

        public UsersController(IValidator<UserRegisterDto> validator, ILogger<UsersController> logger, IMapper mapper, IUserRepository users, UserManager<User> userManager)
        {
            _validator = validator;
            _logger = logger;
            _mapper = mapper;
            _users = users;
            _userManager = userManager;
        }

        [Route(ActionGet), HttpGet, EnableQuery]
        public IQueryable<User> Get()
            => _users.GetQuery();
        
        [HttpPost, Route(ActionRegister)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto, [FromHeader] string password)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                
                validation.AddToModelState(ModelState);
                return UnprocessableEntity(ModelState);
            }

            var user = _mapper.Map<UserRegisterDto, User>(dto);

            var register = await _users.TryRegister(user, password);
            if (register.Succeeded)
            {
                await _users.DirectSignIn(user);
                var response = _mapper.Map<User, UserResponseDto>(user);
                return CreatedAtAction(ActionRegister, response);
            }
            else
            {
                foreach (var err in register.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }

                return BadRequest(ModelState);

            }
        }

    }
}
