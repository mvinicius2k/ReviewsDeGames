using Microsoft.AspNetCore.Identity;
using ReviewsDeGames.Database;
using ReviewsDeGames.Models;
using ReviewsDeGames.Services;

namespace ReviewsDeGames.Repository
{
    public class RolesRepository : IRolesRepository
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly ReviewGamesContext _context;
        private readonly IDescribesService _describes;

        public RolesRepository(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, ReviewGamesContext context, IDescribesService describes)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _describes = describes;
        }

        public Task<IList<string>> GetByUserId(string userId)
        {
            var user = _context.Users.Find(userId) ?? throw new KeyNotFoundException(_describes.KeyNotFound(userId));
            return _userManager.GetRolesAsync(user);
        }

        public async Task<IdentityResult> Put(string role, string userId)
        {
            var user = _context.Users.Find(userId) ?? throw new KeyNotFoundException(_describes.KeyNotFound(userId));
            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
                await _roleManager.CreateAsync(new IdentityRole(role));
            return await _userManager.AddToRoleAsync(user, role);
        }

        public Task<IdentityResult> Remove(string role, string userId)
        {
            var user = _context.Users.Find(userId) ?? throw new KeyNotFoundException(_describes.KeyNotFound(userId));
            return _userManager.RemoveFromRoleAsync(user, role);
            
        }

        

        
            
    }
}
