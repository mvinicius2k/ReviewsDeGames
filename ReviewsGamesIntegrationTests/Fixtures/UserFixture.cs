using FluentAssertions;
using ReviewsDeGames.Controllers;
using ReviewsDeGames.Models;
using ReviewsGamesIntegrationTests.Fakers;
using ReviewsGamesIntegrationTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
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
