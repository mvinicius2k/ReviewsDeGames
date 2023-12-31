﻿//Usar testes em paralelo pode ocasionará em erros


using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using ReviewsDeGames.Controllers;
using ReviewsDeGames.Helpers;
using ReviewsDeGames.Models;
using ReviewsGamesIntegrationTests.Fakers;
using ReviewsGamesIntegrationTests.Fixtures;
using ReviewsGamesIntegrationTests.Helpers;
using ReviewsGamesTests.Fakers;
using ReviewsGamesTests.Fixtures;
using ReviewsGamesTests.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Xunit;
using static System.Net.WebRequestMethods;

namespace ReviewsGamesTests.Integration
{


    public class UserTest : IClassFixture<WebFactory>, IClassFixture<UserFixture>, IClassFixture<AvatarsSet>, IAsyncLifetime
    {
        
        private readonly WebFactory _web;
        private readonly UserFixture _userFixture;
        private readonly AvatarsSet _avatarsSet;
        private readonly UserFaker _userFaker = new UserFaker();

        public UserTest(WebFactory web, UserFixture userFixture, AvatarsSet avatarsSet)
        {


            _web = web;
            _userFixture = userFixture;
            _avatarsSet = avatarsSet;
        }

        [Fact]
        public async Task Get_QueryUsers_ShouldSucess()
        {
            var pageSize = 5;
            var postFaker = new PostFaker();
            for (int i = 0; i < pageSize*2; i++)
            {
                var userData = await _userFixture.Create(_userFaker.Generate(), "passwrdValido10", _web);
                var section = _userFixture.Sections[userData];
                await QuickCreate.PostPost(section, postFaker.Generate());

            }
            var excludeAdmin = $"$filter={nameof(User.NormalizedUserName)} ne 'ADMIN'";
            var expand = nameof(User.Posts); 
            var orderBy = nameof(User.NormalizedUserName);
            var query = $"$orderby={orderBy}&$skip=0&$top={pageSize}&$expand={expand}&{excludeAdmin}";
            var http = _web.Instance.CreateClient();
            var endpoint = EndPoints.Resolve<UsersController>(UsersController.ActionGet) + "?" + query;
            var response = await http.GetAsync(endpoint);
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var results = await response.Content.ReadFromJsonAsync<User[]>();

            results.Should().NotContain(x => x.NormalizedUserName == "ADMIN");
            results.Should().BeInAscendingOrder(u => u.NormalizedUserName);
            results.All(u => u.Posts.Any()).Should().BeTrue();
            results.Should().HaveCount(pageSize);


        }

        [Fact]
        public async Task Get_All_ShouldReturn200WithoutPassowordHash()
        {
            var endpoint = EndPoints.Resolve<UsersController>(UsersController.ActionGet);
            var http = _web.Instance.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            var response = await http.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var users = await response.Content.ReadFromJsonAsync<User[]>();
            users.Should().NotBeNull();
            users.All(u => u.PasswordHash == "secret").Should().BeTrue();
        }


        [Theory, MemberData(nameof(CorrectUsersParams))]
        public async Task Register_CorrectUsers_ShouldReturn201(UserRegisterDto dto, string password)
        {
            //Arrange
            var http = _web.Instance.CreateClient();
            var endpoint = EndPoints.Resolve<UsersController>(UsersController.ActionRegister);
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            //var avatarId = _imagesFixture.PickRandom().Id;
            //dto = dto with { AvatarId = avatarId }; //Copiando dto e inserindo chave estrangeira

            request.Headers.Add(UsersController.PostRegisterPassHeader, password);
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
            var endpoint = EndPoints.Resolve<UsersController>(UsersController.ActionRegister);
            var http = _web.Instance.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);

            request.Headers.Add(UsersController.PostRegisterPassHeader, password);
            request.Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            //Act
            var response = await http.SendAsync(request);

            //Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.UnprocessableEntity, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Register_RepeatedUser_ShouldFail()
        {
            //Arrange
            var http = _web.Instance.CreateClient();
            var endpoint = EndPoints.Resolve<UsersController>(UsersController.ActionRegister);
            var requestSucess = new HttpRequestMessage(HttpMethod.Post, endpoint);
            var requestFail = new HttpRequestMessage(HttpMethod.Post, endpoint);
            
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
            responseFail.StatusCode.Should().BeOneOf(HttpStatusCode.UnprocessableEntity, HttpStatusCode.BadRequest);


        }



        [Fact]
        public async Task PatchInfos_Valid_ShouldReturn200()
        {
            //Arrange
            //Obtendo usuário logado
            UserData loggedUser = await _userFixture.GetOrCreate(_web) ?? throw new Exception("Falha ao obter user");
            var http = _userFixture.Sections[loggedUser];
            var faker = new Faker();
            var files = new FileInfo[] { faker.PickRandom(_avatarsSet.Images.ToArray()) };
            var imageDto = await QuickCreate.PostImage(http, files);
            if (!imageDto.Any())
                Assert.Fail("Não foi possível adicionar um avatar");
            var newInfos = new UserRegisterDto
            {
                AvatarId = imageDto.First().Id,
                Email = "meu-email@gmail.com",
                UserName = "Um.novo.nick"
            };
            var oldInfos = loggedUser.Dto;
            var endPoint = EndPoints.Resolve<UsersController>(UsersController.ActionPatchInfos).Placeholder(loggedUser.Dto.Id);
            var request = new HttpRequestMessage(HttpMethod.Patch, endPoint);
            request.Content = new StringContent(JsonSerializer.Serialize(newInfos), Encoding.UTF8, MediaTypeNames.Application.Json);

            //Act
            var response = await http.SendAsync(request);

            //Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseUser = await response.Content.ReadFromJsonAsync<UserResponseDto>();
            responseUser.Should().NotBeEquivalentTo(oldInfos);

        }

        
        [Fact]
        public async Task PatchInfos_Invalid_ShouldFail()
        {
            //Arrange
            var loggedUser = await _userFixture.GetOrCreate(_web) ?? throw new Exception("Falha ao obter user");
            var http = _userFixture.Sections[loggedUser];
            var newInfos = new UserRegisterDto
            {
                AvatarId = int.MaxValue,
                Email = "emailruim#gmail,com",
                UserName = "   "
            };
            var endPoint = EndPoints.Resolve<UsersController>(UsersController.ActionPatchInfos).Placeholder(loggedUser.Dto.Id);
            var request = new HttpRequestMessage(HttpMethod.Patch, endPoint);
            request.Content = new StringContent(JsonSerializer.Serialize(newInfos), Encoding.UTF8, MediaTypeNames.Application.Json);

            //Act
            var response = await http.SendAsync(request);

            //Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().BeOneOf(HttpStatusCode.UnprocessableEntity, HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task PatchPassword_Valid_ShouldReturn200()
        {
            //Arrange
            var loggedUser = await _userFixture.GetOrCreate(_web) ?? throw new Exception("Sem user");
            var validPassword = "senhaAceitável455";
            var endPoint = EndPoints.Resolve<UsersController>(UsersController.ActionPatchPassword, loggedUser.Dto.Id);
            var request = new HttpRequestMessage(HttpMethod.Patch, endPoint);
            var http = _userFixture.Sections[loggedUser];
            request.Headers.Add(UsersController.PatchPasswordNewPasswordHeader, validPassword);
            request.Headers.Add(UsersController.PatchPasswordCurrentPasswordHeader, loggedUser.Password);

            var verifyEndPoint = EndPoints.Resolve<UsersController>(UsersController.ActionVerifyPassword).Placeholder(loggedUser.Dto.Id);
            var verificationRequest = new HttpRequestMessage(HttpMethod.Get, verifyEndPoint);
            verificationRequest.Headers.Add(UsersController.VerifyPasswordPassHeader, validPassword);

            //Act
            var response = await http.SendAsync(request);
            var verificationResponse = await http.SendAsync(verificationRequest);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            verificationResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var verificationResult = await verificationResponse.Content.ReadFromJsonAsync<PasswordVerificationDto>();
            verificationResult.Result.Should().BeTrue();
            if (verificationResult.Result)
                loggedUser.Password = validPassword;
        }
        [Fact]
        public async Task PatchPassword_OtherUser_ShouldFail()
        {
            //Arrange
            var userA = await _userFixture.GetOrCreate(_web);
            var userB = await _userFixture.Create(new UserFaker().Generate(), "algumPass1230", _web);
            var endPoint = EndPoints.Resolve<UsersController>(UsersController.ActionPatchPassword).Placeholder(userB.Dto.Id);
            var request = new HttpRequestMessage(HttpMethod.Patch, endPoint);
            var http = _userFixture.Sections[userA];
            request.Headers.Add(UsersController.PatchPasswordNewPasswordHeader, "algumaSenha2355");
            request.Headers.Add(UsersController.PatchPasswordCurrentPasswordHeader, userA.Password);

            //Act
            var response = await http.SendAsync(request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        }



        [Fact]
        public async Task Delete_WithoutRole_ShouldFail()
        {
            var loggedUser = await _userFixture.GetOrCreate(_web);
            var firstUserSection = _userFixture.Sections[loggedUser];
            var otherUser = await _userFixture.Create(_userFaker.Generate(), "senhaValida123", _web);
            var endPoint = EndPoints.Resolve<UsersController>(UsersController.ActionDelete).Placeholder(otherUser.Dto.Id);
            var request = new HttpRequestMessage(HttpMethod.Delete, endPoint);
            request.Headers.Add(UsersController.DeletePasswordHeader, loggedUser.Password);

            var response = await firstUserSection.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        [Fact]
        public async Task Delete_Self_ShouldSucess()
        {
            var loggedUser = await _userFixture.GetOrCreate(_web);
            var endPoint = EndPoints.Resolve<UsersController>(UsersController.ActionDelete).Placeholder(loggedUser.Dto.Id);
            var request = new HttpRequestMessage(HttpMethod.Delete, endPoint);
            request.Headers.Add(UsersController.DeletePasswordHeader, loggedUser.Password);
            var http = _userFixture.Sections[loggedUser];

            var response = await http.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Delete_OtherWithPostsAsAdmin_ShouldReturn200()
        {
            var postsCount = 4;
            var otherUser = await _userFixture.GetOrCreate(_web);
            var admin = await _userFixture.GetAdmin(_web);
            var adminSection = _userFixture.Sections[admin];
            var postsFaker = new PostFaker();
            var posts = postsFaker.Generate(4);
            var otherUserSection = _userFixture.Sections[otherUser];
            foreach (var post in posts)
            {
                
                var postAdded = await QuickCreate.PostPost(otherUserSection, post);
                await QuickCreate.PutVote(adminSection, new UserVoteRequestDto
                {
                    PostId = postAdded.Id,
                    Value = 10, //Valor arbitrário
                });
            }

            //Act
            var endPoint = EndPoints.Resolve<UsersController>(UsersController.ActionDelete, otherUser.Dto.Id);
            var request = new HttpRequestMessage(HttpMethod.Delete, endPoint);
            request.Headers.Add(UsersController.DeletePasswordHeader, admin.Password);
            var response = await adminSection.SendAsync(request);
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
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
            var fakerWithBadNickname = new UserFaker();
            var fakerWithBadAvatarId = new UserFaker();

            fakerWithBadEmail.RuleFor(u => u.Email, f => f.PickRandom(new string[]
            {
                "emailruim", "sdfjghflk@", "     ", string.Join("",f.Lorem.Words(300)) + "@gmail.com", "ruim@hotmail.com@hotmail.com", ""
            }));
           
            fakerWithBadNickname.RuleFor(u => u.UserName, f => f.PickRandom(new string[]
            {
                "   ", "", f.Internet.Email(), f.Random.String(300, 'a', 'z'), string.Join("", f.Random.Shuffle(f.Random.String(10, 'a','z') + "@")),
            }));
            fakerWithBadNickname.RuleFor(u => u.AvatarId, f => f.Random.Int());

            var faker = new Faker();
            var badPasswords = new string[] {
                "sóletras", "154789456", "a", "   ", "", ".", faker.Lorem.Paragraph(), string.Join("", faker.Random.Shuffle(faker.Random.String(10, 'a', 'z') + "@"))
            };

            var users = new List<UserRegisterDto>();
            users.AddRange(fakerWithBadEmail.Generate(10));
            users.AddRange(fakerWithBadNickname.Generate(10));
            users.AddRange(fakerWithBadAvatarId.Generate(3));




            foreach (var item in users)
            {
                yield return new object[] { item, faker.PickRandom(badPasswords) };
            }
        }

        public async  Task InitializeAsync()
        {
            var imagesSetSize = 10;
            var loggedUser = await _userFixture.GetOrCreate(_web);
            var http = _userFixture.Sections[loggedUser];
            _avatarsSet.ReadPath();
            await _avatarsSet.TryPopulate(imagesSetSize);
            if (!_avatarsSet.Images.Any())
                Assert.Fail($"Pasta de imagens {_avatarsSet.ImageFolder} está vazia");


        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
        #endregion
    }
}

