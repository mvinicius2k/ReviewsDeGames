using Azure;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using ReviewsDeGames.Helpers;
using ReviewsDeGames.Models;
using ReviewsDeGames.Repository;

namespace ReviewsDeGames.Database
{
    public class DbInit 
    {
        private static readonly User AdminUser = new User
        {
            Email = "admin@email.com",
            AvatarId = null,
            UserName = "Admin",
        };
        private static readonly string AdminPassword = "Admin1234";

        private readonly ILogger<DbInit> _logger;
        private readonly ReviewGamesContext _context;
        private readonly IUserRepository _users;

        public DbInit(ILogger<DbInit> logger, ReviewGamesContext context, IUserRepository users)
        {
            _logger = logger;
            _context = context;
            _users = users;
        }

        public async Task Initialize(bool seed, bool restart)
        {
            try
            {
                if (restart)
                {
                    var result = _context.Database.EnsureDeleted();
                    _logger.LogInformation(result ? "Banco de dados deletado" : "Banco de dados inexistente, nada para deletar");
                }
                _context.Database.EnsureCreated();
            }
            catch (OperationCanceledException e)
            {
                _logger.LogError(e, "Erro ao criar DB");
                throw;
            }
            

          
            if (!seed)
                return;

            if(!_context.Users.Any())
                await _users.TryRegister(AdminUser, AdminPassword);

            await _users.AddRoleToUser(AdminUser, Values.RoleAdmin);




            _logger.LogInformation("Banco de dados conectado com sucesso");
        }
    }
}
