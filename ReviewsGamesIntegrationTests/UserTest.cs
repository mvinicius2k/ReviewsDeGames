using Azure.Core;
using FluentAssertions;
using ReviewsDeGames.Controllers;
using ReviewsDeGames.Models;
using ReviewsGamesIntegrationTests.Fakers;
using ReviewsGamesIntegrationTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReviewsGamesIntegrationTests
{
    public record UserData : IDisposable
    {
        public string Password { get; init; }
        public UserResponseDto Dto { get; init; }
        public HttpClient HttpClient { get; init; }

        public void Dispose()
            => HttpClient.Dispose();
    }

    public class UserTest : IClassFixture<WebFactory>, IAsyncLifetime
    {
        #region EndPoints
        private readonly static string RegisterEndPoint = string.Join(',', UsersController.Route, UsersController.ActionRegister);
        #endregion

        private UserData LoggedUser { get; set; }
        private readonly WebFactory _web;



        public UserTest(WebFactory web)
        {
            _web = web;
        }

        [Theory, MemberData(nameof(CorrectUsersParams))]
        public async Task Register_CorrectUsers_ShouldReturn201(UserRegisterDto dto)
        {
            //Arrange
            var http = _web.Instance.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, RegisterEndPoint);
            request.Headers.Add(UsersController.PostRegisterPassHeader, "senhaBoa2182");
            request.Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8);
            //Act
            var response = await http.SendAsync(request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created); 

        }


        public Task DisposeAsync()
        {
            LoggedUser.Dispose();
            return Task.CompletedTask;
        }

        public async Task InitializeAsync()
        {
            var fakeUser = new UserFaker();
            var user = fakeUser.Generate();
            var http = _web.Instance.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, RegisterEndPoint);
            request.Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8);
            var pass = "Senharuim1234";
            request.Headers.Add(UsersController.PostRegisterPassHeader, pass);

            var response = await http.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                LoggedUser = new UserData
                {
                    Dto = await response.Content.ReadFromJsonAsync<UserResponseDto>(),
                    HttpClient = http,
                    Password = pass
                };
            }
        }

        #region MemberData
        public static IEnumerable<object[]> ChangePasswordParams()
        {
            var faker = 

            var paramsData = new object[][]
            {
                new object[] {  },
                new object[] { "1234senhameh", HttpStatusCode.OK },
                new object[] { "", HttpStatusCode.BadRequest },
                new object[] { "   ", HttpStatusCode.BadRequest },
                new object[] { "@#%$#$%#$%#%$", HttpStatusCode.UnprocessableEntity },
                new object[] { "@#%$a#$%#-1234", HttpStatusCode.OK },
            };

            foreach (var item in paramsData)
                yield return item;
        }
    }
}
