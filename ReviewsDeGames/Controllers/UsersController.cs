﻿using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using ReviewsDeGames.Helpers;
using ReviewsDeGames.Models;
using ReviewsDeGames.Repository;
using ReviewsDeGames.Services;
using ReviewsDeGames.Validators;
using System.Security.Claims;

namespace ReviewsDeGames.Controllers
{
    [ApiController, Route(Route)]
    public class UsersController : ControllerBase
    {
        #region Constants

        public const string Route = "Users";

        public const string ActionRegister = "register";
        public const string ActionPatchAvatar = "patchAvatar";
        public const string ActionLogin = "login";
        public const string ActionLogout = "logout";
        public const string ActionPatchPassword = $"patchPassword/{{{PatchPasswordUserIdRoute}}}";
        public const string ActionPatchInfos = $"patchInfos/{{{PatchInfosUserIdRoute}}}";
        public const string ActionVerifyPassword = $"verifyPassword/{{{VerifyPasswordIdRoute}}}";
        public const string ActionGet = "get";
        public const string ActionDelete = $"delete/{{{DeleteIdRoute}}}";

        public const string LoginLoginHeader = "login";
        public const string LoginPasswordHeader = "password";
        public const string LoginRememberHeader = "remember";
        public const string PostRegisterPassHeader = "pass";
        public const string PatchAvatarIdHeader = "Id";
        public const string PatchAvatarUrlHeader = "Avatar-Url";
        public const string PatchPasswordUserIdRoute = "id";
        public const string PatchPasswordCurrentPasswordHeader = "Current-Pass";
        public const string PatchPasswordNewPasswordHeader = "New-Pass";
        public const string PatchInfosUserIdRoute = "id";
        public const string VerifyPasswordPassHeader = "password";
        public const string VerifyPasswordIdRoute = "id";
        public const string DeleteIdRoute = "id";
        public const string DeletePasswordHeader = "password";

        #endregion

        private readonly IValidator<UserRegisterDto> _validator;
        private readonly ILogger<UsersController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserRepository _users;
        private readonly IDescribesService _describes;
        private readonly IRolesRepository _roles;

        public UsersController(IValidator<UserRegisterDto> validator, ILogger<UsersController> logger, IMapper mapper, IUserRepository users, IDescribesService describes, IRolesRepository roles)
        {
            _validator = validator;
            _logger = logger;
            _mapper = mapper;
            _users = users;
            _describes = describes;
            _roles = roles;
        }
        /// <summary>
        /// Obtém uma consulta de <see cref="User"/> com base numa query OData passada pela url
        /// </summary>
        /// <response code="200">Sucesso</response>
        [Route(ActionGet), HttpGet, EnableQuery]
        public IQueryable<User> Get()
            => _users.GetQuery();

        
        /// <summary>
        /// Registra um usuário comum
        /// </summary>
        /// <response code="201">Sucesso</response>
        /// <response code="422">Caso seja verificado algum dado errado previamente</response>
        /// <response code="400">Caso aconteça algum erro ao criar em decorrência de algum dado inválido</response>
        /// <param name="pass"> Senha que será processada para um hash </param>
        [HttpPost, Route(ActionRegister)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto, [FromHeader(Name = PostRegisterPassHeader)] string pass)
        {
            //Ruleset de validação para novo usuário
            var rulesets = new string[] { UserValidator.AddNewRuleSet, "default" };
            var validation = await _validator.ValidateAsync(dto, opt => opt.IncludeRuleSets(rulesets));
            if (!validation.IsValid)
            {

                validation.AddToModelState(ModelState);
                return UnprocessableEntity(ModelState);
            }

            var user = _mapper.Map<UserRegisterDto, User>(dto);

            var register = await _users.TryRegister(user, pass);
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

        /// <summary>
        /// Faz o login adicionando um cookie do identity na sessão
        /// </summary>
        /// <response code="201">Sucesso</response>
        /// <response code="401"> O login ou senha estão incorretos </response>
        [HttpPost, Route(ActionLogin)]
        public async Task<IActionResult> Login([FromHeader(Name = LoginLoginHeader)] string login, [FromHeader(Name = LoginPasswordHeader)] string pass, [FromHeader(Name = LoginRememberHeader)] bool rememberMe)
        {
            try
            {

                var verification = await _users.SignIn(login, pass, rememberMe);
                if (verification.Succeeded)
                {
                    var user = _users.GetQuery().First(x => x.NormalizedUserName == login.ToUpper());
                    var response = _mapper.Map<UserResponseDto>(user);
                    return Ok(response);
                }
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogDebug(e.StackTrace);
                ModelState.AddModelError("LoginFail", _describes.KeyNotFound(login));
            }


            return Unauthorized(ModelState);

        }

        /// <response code="200">Sucesso</response>
        [HttpPost, Route(ActionLogout)]
        public async Task<IActionResult> Logout()
        {
            await _users.SignOut();
            return Ok();

        }

        /// <summary>
        /// Edita as informações do usuário, menos a senha
        /// </summary>
        /// <response code="200">Sucesso</response>
        /// <response code="401">Caso o usuário a editar seja um outro que não o logado ou aconteça algum erro de autenticação</response>
        /// <response code="401">Usuário não encontrado</response>
        /// <response code="422">Algum dado do modelo é inválido</response>
        [HttpPatch, Route(ActionPatchInfos), Authorize]
        public async Task<IActionResult> PatchInfos([FromRoute(Name = PatchInfosUserIdRoute)] string id,[FromBody] UserRegisterDto dto)
        {
            var rulesets = new string[] { "default", UserValidator.UpdateRuleSet };

            //Verificando se nao está alterando outro user ou um nao existete 
            var userIdRequest = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdRequest != id)
                return Unauthorized();
            else if (userIdRequest == null)
                return NotFound(_describes.KeyNotFound(id));

            //Validando entrada
            var validation = await _validator.ValidateAsync(dto, opt => opt.IncludeRuleSets(rulesets));
            if (!validation.IsValid)
            {
                validation.AddToModelState(ModelState);
                return UnprocessableEntity(ModelState);
            }

            //Obtendo usuário completo
            var user = _users.GetQuery().AsNoTracking().First(u => u.Id == id);

            //Aplicando patch
            user = _mapper.Map<UserRegisterDto, User>(dto, user);
            await _users.HardUpdate(id, user);
            var response = _mapper.Map<User, UserResponseDto>(user);
            return Ok(response);
        }

        /// <summary>
        /// Edita somente a senha
        /// </summary>
        /// <response code="200">Sucesso</response>
        /// <response code="401">Algum erro de permissão ou a atual senha passada é incorreta</response>
        [HttpPatch, Route(ActionPatchPassword), Authorize]
        public async Task<IActionResult> PatchPassowrd([FromRoute(Name = PatchPasswordUserIdRoute)] string userId, [FromHeader(Name = PatchPasswordCurrentPasswordHeader)] string currentPassword, [FromHeader(Name = PatchPasswordNewPasswordHeader)] string newPassoword)
        {
            //Verificando se nao está alterando outro user
            var userIdRequest = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdRequest != userId)
                return Unauthorized();


            var result = await _users.UpdatePassword(userId, currentPassword, newPassoword);

            if (result.Succeeded)
                return Ok();

            foreach (var err in result.Errors)
                ModelState.AddModelError(err.Code, err.Description);

            return Unauthorized(ModelState);
        }

        /// <response code="200">Sucesso</response>
        [HttpGet, Route(ActionVerifyPassword)]
        public async ValueTask<IActionResult> VerifyPassword([FromRoute(Name = VerifyPasswordIdRoute)] string userId, [FromHeader(Name = VerifyPasswordPassHeader)] string pass)
        {
            var result = await _users.VerifyPassword(userId, pass);
            return Ok(new PasswordVerificationDto { Result = result});
        }

        /// <summary>
        /// Deleta o usuário e todas as entidades dependentes dele em cascata
        /// </summary>
        /// <response code="200">Sucesso</response>
        /// <response code="401">Erro de permissão</response>
        /// <response code="400">Algum dado passado é incorreto</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpDelete,Route(ActionDelete), Authorize]
        public async Task<IActionResult> Delete([FromRoute(Name = DeleteIdRoute)] string id, [FromHeader(Name = DeletePasswordHeader)] string password)
        {
            //Verificando se nao está alterando outro user
            var userIdRequest = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRoles = await _roles.GetByUserId(userIdRequest);
            var passwordVerification = await _users.VerifyPassword(userIdRequest, password);
            var isAdmin = userRoles.Contains(Values.RoleAdmin);



            
            if (!passwordVerification || (userIdRequest != id && !isAdmin))
                return Unauthorized();

            try
            {
                var result = await _users.Delete(id);
                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                        ModelState.AddModelError(err.Code, err.Description);
                    
                    return BadRequest(ModelState);
                }

                return Ok();
            }
            catch (KeyNotFoundException)
            {
                ModelState.AddModelError(nameof(_describes.KeyNotFound), _describes.KeyNotFound(id));
                return NotFound(ModelState);
                
            }
        }

    }
}
