using Microsoft.AspNetCore.Identity;

namespace ReviewsDeGames.Repository
{
    public interface IRolesRepository
    {
        public Task<IList<string>> GetByUserId(string userId);
        public Task<IdentityResult> Put(string role, string userId);
        public Task<IdentityResult> Remove(string role, string userId);
    }
}
