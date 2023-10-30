using Microsoft.AspNetCore.Identity;
using ReviewsDeGames.Models;

namespace ReviewsDeGames.Repository
{
    public interface IUserRepository
    {
        public Task<IdentityResult> TryRegister(User user, string rawPassowrd);
        public Task DirectSignIn(User user, bool isPersistent = false);
        public Task<SignInResult> SignIn(string nickname, string password, bool rememberMe);
        public Task SignOut();
        public Task<User> PatchAvatar(string userId, string? imagePath);

        /// <exception cref="KeyNotFoundException"/>
        public Task<IdentityResult> UpdatePassword(string id, string currentPassword, string newPassword);
        /// <exception cref="KeyNotFoundException"/>
        public Task HardUpdate(string id, User user);
        public ValueTask<bool> VerifyPassword(string userId, string password);
        public IQueryable<User> GetQuery();
        public Task<IdentityResult> Delete(string id);
        public Task<IdentityResult> AddRoleToUser(User user, string role);
        public Task<IdentityResult> RemoveRoleFromUser(User user, string role);

    }
}
