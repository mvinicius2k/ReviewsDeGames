using Azure;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using ReviewsDeGames.Helpers;

namespace ReviewsDeGames.Database
{
    public class DbInit 
    {
        private readonly ILogger<DbInit> _logger;
        private readonly ReviewGamesContext _context;

        public DbInit(ILogger<DbInit> logger, ReviewGamesContext context)
        {
            _logger = logger;
            _context = context;
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
            _logger.LogInformation("Banco de dados conectado com sucesso");
        }
    }
}
