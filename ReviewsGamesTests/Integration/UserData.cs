//Usar testes em paralelo pode ocasionar em erros


using ReviewsDeGames.Models;

namespace ReviewsGamesTests.Integration
{
    /// <summary>
    /// Superconjunto de dados de usuário incluindo a senha
    /// </summary>
    public class UserData
    {
        public string Password { get; set; }
        public UserResponseDto Dto { get; set; }


    }
}

