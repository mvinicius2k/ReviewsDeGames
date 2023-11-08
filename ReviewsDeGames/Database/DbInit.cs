using Azure;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using ReviewsDeGames.Helpers;
using ReviewsDeGames.Models;
using ReviewsDeGames.Repository;

namespace ReviewsDeGames.Database
{
    /// <summary>
    /// Faz o seed e registra algumas entidades  padrões para o sistema
    /// </summary>
    public class DbInit
    {

        

        private readonly ILogger<DbInit> _logger;
        private readonly ReviewGamesContext _context;
        private readonly IUserRepository _users;
        private readonly IRolesRepository _roles;

        public DbInit(ILogger<DbInit> logger, ReviewGamesContext context, IUserRepository users, IRolesRepository roles)
        {
            _logger = logger;
            _context = context;
            _users = users;
            _roles = roles;
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

            if (!_context.Users.Any())
                await _users.TryRegister(Values.AdminUser, Values.AdminPassword);

            await _roles.Put(Values.RoleAdmin, Values.AdminUser.Id);




            _logger.LogInformation("Banco de dados conectado com sucesso");
        }
    }
}
