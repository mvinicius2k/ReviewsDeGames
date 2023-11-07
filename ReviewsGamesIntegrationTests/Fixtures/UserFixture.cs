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

    public class UserFixture
    {
        public Dictionary<UserData, HttpClient> Sections { get; private set; }
        
        public UserFixture()
        {
            Sections = new Dictionary<UserData, HttpClient>();
        }

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

        public async Task<UserData> GetAdmin(WebFactory web)
        {
            //var query = $"$filter={nameof(User.NormalizedUserName)} eq 'ADMIN'";
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


        //public async Task<UserData> GetOrCreate(UserData user) 
        //{
        //    if (Sections.ContainsKey(user))
        //        return user;

        //    var http = _factory.Instance.CreateClient();
        //    var request = new HttpRequestMessage(HttpMethod.Post, EndPoints.UserRegister);
        //    request.Headers.Add(UsersController.PostRegisterPassHeader, user.Password);
        //    request.Content = new StringContent(JsonSerializer.Serialize(user.Dto), Encoding.UTF8, "application/json");
        //    //Act
        //    var response = await http.SendAsync(request);

        //    var dto = await response.Content.ReadFromJsonAsync<UserResponseDto>();

        //    var newUser = new UserData
        //    {
        //        Dto = dto,
        //        Password = user.Password
        //    };
        //    Sections[newUser] = http;
        //    return newUser;

        //}

        public void Dispose()
        {
            
        }
    }
}
