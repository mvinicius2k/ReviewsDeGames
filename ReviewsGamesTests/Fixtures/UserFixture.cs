using FluentAssertions;
using ReviewsDeGames.Controllers;
using ReviewsDeGames.Helpers;
using ReviewsDeGames.Models;
using ReviewsGamesIntegrationTests.Fakers;
using ReviewsGamesIntegrationTests.Helpers;
using ReviewsGamesTests.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReviewsGamesIntegrationTests.Fixtures
{
    /// <summary>
    /// Guarda dados e sessões de usuários. Os dados precisam ser atualizados manualmente no <see cref="Sections"/> caso alterados
    /// </summary>
    public class UserFixture
    {
        public Dictionary<UserData, HttpClient> Sections { get; private set; }
        
        public UserFixture()
        {
            Sections = new Dictionary<UserData, HttpClient>();
        }

        /// <summary>
        /// Envia uma usuário para o endpoint de criação de usuário, processa a resposta e devolve já objeto. <br/>
        /// Além disso a sessão é armazenada em <see cref="Sections"/>
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="web"></param>
        /// <returns></returns>
        public async Task<UserData?> Create(UserRegisterDto user, string password, WebFactory web)
        {
            var http = web.Instance.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Post, EndPoints.Resolve<UsersController>(UsersController.ActionRegister));
            request.Headers.Add(UsersController.PostRegisterPassHeader, password);
            request.Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = await http.SendAsync(request);
            var userResponse = await response.Content.ReadFromJsonAsync<UserResponseDto>();
            if (userResponse == null)
                return null;
            var userData = new UserData
            {
                Dto = userResponse,
                Password = password,
            };
            Sections[userData] = http;
            return userData;
            

        }

        /// <summary>
        /// Obtém um usuário qualquer já registrado ou cria um novo e o armazena no conjunto
        /// </summary>
        /// <param name="web"></param>
        /// <returns></returns>
        public async Task<UserData> GetOrCreate(WebFactory web)
        {
            if (Sections.Count == 0)
            {
                var userToAdd = new UserFaker().Generate();
                var addedUser = await Create(userToAdd, "senhaVálida1234", web);
                if (addedUser == null)
                    Assert.Fail("Não foi possível adicionar o usuário");
                return addedUser;
            }
            else
                return Sections.Keys.First();
        }

        /// <summary>
        /// Obtém ou loga no usuário padrão de permissões admin. <br/>
        /// [!] Caso o sistema não tenha sido 'seedado' corretamente, a operação falhará
        /// </summary>
        /// <param name="web"></param>
        /// <returns></returns>
        public async Task<UserData> GetAdmin(WebFactory web)
        {
            var endpoint = EndPoints.Resolve<UsersController>(UsersController.ActionLogin);
            var client = web.Instance.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Add(UsersController.LoginLoginHeader, Values.AdminUser.UserName);
            request.Headers.Add(UsersController.LoginPasswordHeader, Values.AdminPassword);
            request.Headers.Add(UsersController.LoginRememberHeader, "true");
            var response = await client.SendAsync(request);
            
            var dto = await response.Content.ReadFromJsonAsync<UserResponseDto>();

            
                
            var userData = new UserData
            {
                Dto = dto,
                Password = Values.AdminPassword
            };

            Sections.Add(userData, client);
            return userData;
        }


       
    }
}
