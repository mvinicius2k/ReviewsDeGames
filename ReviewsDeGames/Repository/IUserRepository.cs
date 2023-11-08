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
        /// <summary>
        /// Atualiza somente o avatar
        /// </summary>
        public Task<User> PatchAvatar(string userId, int? imageId);

        /// <summary>
        /// Atualiza somente a senha
        /// </summary>
        /// <exception cref="KeyNotFoundException"/>
        public Task<IdentityResult> UpdatePassword(string id, string currentPassword, string newPassword);
        /// <summary>
        /// Atualiza toda uma entidade <see cref="User"/> sem fazer nenhuma verificação
        /// </summary>
        /// <exception cref="KeyNotFoundException"/>
        public Task HardUpdate(string id, User user);
        public ValueTask<bool> VerifyPassword(string userId, string password);
        /// <summary>
        /// Obtém um objeto de query para usar linq ou retornar como OData
        /// </summary>
        public IQueryable<User> GetQuery();
        public Task<IdentityResult> Delete(string id);
     

    }
}
