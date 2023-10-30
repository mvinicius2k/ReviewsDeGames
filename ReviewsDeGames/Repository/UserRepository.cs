using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using ReviewsDeGames.Database;
using ReviewsDeGames.Helpers;
using ReviewsDeGames.Models;
using ReviewsDeGames.Services;
using System.Net.Mail;

namespace ReviewsDeGames.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ReviewGamesContext _context;
        private readonly IDescribesService _describes;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRepository(ILogger<UserRepository> logger, UserManager<User> userManager, SignInManager<User> signInManager, ReviewGamesContext context, IDescribesService describes, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _describes = describes;
            _roleManager = roleManager;
        }

        public Task DirectSignIn(User user, bool isPersistent = false)
            => _signInManager.SignInAsync(user, isPersistent);

        public IQueryable<User> GetQuery()
            => _context.Users.AsQueryable();

        public async Task<User> PatchAvatar(string userId, string? imagePath)
        {
            var user = _context.Users.Find(userId) ?? throw new KeyNotFoundException(_describes.KeyNotFound(userId));

            user.AvatarUrl = imagePath;
            await _context.SaveChangesAsync();

            return user;
        }



        public Task<SignInResult> SignIn(string login, string password, bool rememberMe)
        {
            login = login.ToUpper();
            var isEmail = MailAddress.TryCreate(login, out _);
            if (MailAddress.TryCreate(login, out _))
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == login);
                if (user != null)
                    login = user.NormalizedUserName;
            }

            return _signInManager.PasswordSignInAsync(login, password, rememberMe, false);

        }

        public Task SignOut()
            => _signInManager.SignOutAsync();

        public async Task<IdentityResult> TryRegister(User user, string rawPassowrd)
            => await _userManager.CreateAsync(user, rawPassowrd);
        
        public async Task<IdentityResult> AddRoleToUser(User user, string role)
        {
            var adminRoleExists = await _roleManager.RoleExistsAsync(role);
            if (!adminRoleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }

            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<IdentityResult> RemoveRoleFromUser(User user, string role)
            => await _roleManager.DeleteAsync(new IdentityRole(role));
        

        public Task HardUpdate(string id, User model)
        {
            if(!_context.Users.Any(u => id == u.Id))
                throw new KeyNotFoundException(_describes.KeyNotFound(id));

            model.Id = id;
            return _userManager.UpdateAsync(model);
        }

        public Task<IdentityResult> UpdatePassword(string id, string currentPassword, string newPassword)
        {

            var user = _context.Users.Find(id) ?? throw new KeyNotFoundException(_describes.KeyNotFound(id));
            return _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        }

        public async ValueTask<bool> VerifyPassword(string userId, string password)
        {
            var user = _context.Users.Find(userId) ?? throw new KeyNotFoundException();
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            return result.Succeeded;
        }

        public Task<IdentityResult> Delete(string id)
        {
            var userToDelete = _context.Users.Find(id) ?? throw new KeyNotFoundException(_describes.KeyNotFound(id));
            return _userManager.DeleteAsync(userToDelete);
        }
    }
}
