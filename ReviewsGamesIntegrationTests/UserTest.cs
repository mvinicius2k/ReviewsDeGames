using Azure.Core;
using Bogus;
using FluentAssertions;
using ReviewsDeGames.Controllers;
using ReviewsDeGames.Models;
using ReviewsGamesIntegrationTests.Fakers;
using ReviewsGamesIntegrationTests.Fixtures;
using ReviewsGamesIntegrationTests.Helpers;
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
    public record UserData
    {
        public string Password { get; init; }
        public UserResponseDto Dto { get; init; }

      
    }

    public class UserTest : IClassFixture<WebFactory>, IClassFixture<UserFixture>
    {
        private readonly WebFactory _web;
        private readonly UserFixture _userFixture;
        private readonly UserRegisterDto _userAdmin;


        public UserTest(WebFactory web, UserFixture userFixture)
        {
            _web = web;
            _userFixture = userFixture;
            _userAdmin = new UserFaker().Generate();
        }

        [Fact]
        public async Task Get_ShouldReturn200()
        {
            var http = _web.Instance.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, EndPoints.UserGetAll);
            var response = await http.SendAsync(request);
            response.IsSuccessStatusCode.Should().BeTrue();
        }


        [Theory, MemberData(nameof(CorrectUsersParams))]
        public async Task Register_CorrectUsers_ShouldReturn201(UserRegisterDto dto, string password)
        {
            //Arrange
            var http = _web.Instance.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, EndPoints.UserRegister);
            

            request.Headers.Add(UsersController.PostRegisterPassHeader,password);
            request.Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            //Act
            var response = await http.SendAsync(request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created); 

        }

        [Theory, MemberData(nameof(InvalidUsersParams))]
        public async Task Register_InvalidUsers_ShouldReturnFail(UserRegisterDto dto, string password)
        {
            //Arrange
            var http = _web.Instance.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, EndPoints.UserRegister);

            request.Headers.Add(UsersController.PostRegisterPassHeader, password);
            request.Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            //Act
            var response = await http.SendAsync(request);

            //Assert
            response.StatusCode.Should().NotBe(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Register_RepeatedUser_ShouldFail()
        {
            //Arrange
            var http = _web.Instance.CreateClient();
            var requestSucess = new HttpRequestMessage(HttpMethod.Post, EndPoints.UserRegister);
            var requestFail = new HttpRequestMessage(HttpMethod.Post, EndPoints.UserRegister);
            var faker = new UserFaker();
            var user = faker.Generate();
            var password = "passwordBom1248";

            requestSucess.Headers.Add(UsersController.PostRegisterPassHeader, password);
            requestSucess.Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
            requestFail.Headers.Add(UsersController.PostRegisterPassHeader, password);
            requestFail.Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

            //Act
            var responseSucess = await http.SendAsync(requestSucess);
            var responseFail = await http.SendAsync(requestFail);

            //Assert
            responseSucess.StatusCode.Should().Be(HttpStatusCode.Created);
            responseFail.StatusCode.Should().NotBe(HttpStatusCode.Created);


        }


        [Fact]
        public async Task PatchInfos_Invalid_ShouldFail()
        {

        }
        [Fact]
        public async Task PatchInfos_Valid_ShouldReturn200()
        {

        }
        [Fact]
        public async Task PatchPassword_Valid_ShouldReturn200()
        {

        }
        [Fact]
        public async Task PatchPassword_OtherUser_ShouldFail()
        {

        }
        [Fact]
        public async Task Delete_WithoutRole_ShouldFail()
        {

        }
        [Fact]
        public async Task Delete_Self_ShouldSucess()
        {

        }

        #region MemberData
        public static IEnumerable<object[]> CorrectUsersParams()
        {
            var userFaker = new UserFaker();
            var users = userFaker.Generate(40);
            var passwords = new string[] { "senhaBoa123, frase como senha 10, 145254sd, asder@$#3, ☺dfkmsd35, 148Wdfrg" };
            var faker = new Faker();
            foreach (var item in users)
                yield return new object[] { item, faker.PickRandom(passwords) };
        }
        public static IEnumerable<object[]> InvalidUsersParams()
        {
            var fakerWithBadEmail = new UserFaker();
            var fakerWithBadAvatarUrl = new UserFaker();
            var fakerWithBadNickname = new UserFaker();

            fakerWithBadEmail.RuleFor(u => u.Email, f => f.PickRandom(new string[] 
            {
                "emailruim", "sdfjghflk@", "     ", string.Join("",f.Lorem.Words(300)) + "@gmail.com", "ruim@hotmail.com@hotmail.com", ""
            }));
            fakerWithBadAvatarUrl.RuleFor(u => u.AvatarUrl, f => f.PickRandom(new string[]
            {
                f.Internet.Email(), f.Internet.UrlWithPath("ftp", fileExt: ".jpg"), f.Internet.UrlRootedPath(), "     ", "", 
            }));
            fakerWithBadNickname.RuleFor(u => u.UserName, f => f.PickRandom(new string[]
            {
                "   ", "", f.Internet.Email(), f.Random.String(300, 'a', 'z'), string.Join("", f.Random.Shuffle(f.Random.String(10, 'a','z') + "@")),
            }));


            var faker = new Faker();
            var badPasswords = new string[] {
                "sóletras", "154789456", "a", "   ", "", ".", faker.Lorem.Paragraph(), string.Join("", faker.Random.Shuffle(faker.Random.String(10, 'a', 'z') + "@"))
            };

            var users = new List<UserRegisterDto>();
            users.AddRange(fakerWithBadAvatarUrl.Generate(10));
            users.AddRange(fakerWithBadEmail.Generate(10));
            users.AddRange(fakerWithBadNickname.Generate(10));




            foreach (var item in users)
            {
                yield return new object[] { item, faker.PickRandom(badPasswords) };
            }
        }
        #endregion
    }
}

