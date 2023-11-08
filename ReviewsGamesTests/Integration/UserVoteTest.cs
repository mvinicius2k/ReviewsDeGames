using Bogus;
using FluentAssertions;
using ReviewsDeGames.Controllers;
using ReviewsDeGames.Helpers;
using ReviewsDeGames.Models;
using ReviewsGamesIntegrationTests.Fakers;
using ReviewsGamesIntegrationTests.Fixtures;
using ReviewsGamesIntegrationTests.Helpers;
using ReviewsGamesTests.Fakers;
using ReviewsGamesTests.Fixtures;
using ReviewsGamesTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ReviewsGamesTests.Integration
{
    public class UserVoteTest : IClassFixture<UserFixture>, IClassFixture<ImagesSet>, IClassFixture<WebFactory>, IAsyncLifetime
    {

        private readonly WebFactory _web;
        private readonly UserFixture _userFixture;
        private readonly ImagesSet _imagesSet;
        private readonly PostFaker _postFaker = new();
        private readonly UserFaker _userFaker = new();
        private readonly Faker _faker = new();
        public UserVoteTest(WebFactory web, UserFixture userFixture, ImagesSet imagesSet)
        {
            _web = web;
            _userFixture = userFixture;
            _imagesSet = imagesSet;
        }

        [Fact]
        public async Task Get_QueryPosts_ShouldSucess()
        {
            //Arrange
            var pageSize = 5;
            var user = await _userFixture.GetOrCreate(_web);
            var section = _userFixture.Sections[user];
            

            //Criando users e posts
            var userDtos = _userFaker.Generate(pageSize);
            var sections = new List<HttpClient>(userDtos.Count);
            var createdPosts = new List<Post>(userDtos.Count);
            foreach (var dto in userDtos)
            {
                var userData = await _userFixture.Create(dto, "passValido123", _web);
                var http = _userFixture.Sections[userData];
                sections.Add(http);
                var post = _postFaker.Generate();
                var createdPost = await QuickCreate.PostPost(http, post);
                createdPosts.Add(createdPost);

            }

            //Votanto
            foreach (var post in createdPosts)
            {
                var userToVote = _faker.PickRandom(sections);
                await QuickCreate.PutVote(userToVote, new UserVoteRequestDto
                {
                    PostId = post.Id,
                    Value = _faker.Random.Int(0, 10)
                }) ;
            }


            //Montando query
            var expand = nameof(UserVote.Post);
            var orderBy = nameof(UserVote.Value);
            var query = $"$orderby={orderBy}&$skip=0&$top={pageSize}&$expand={expand}";

            //Act
            var endpoint = EndPoints.Resolve<UserVotesController>(UserVotesController.ActionGet) + "?" + query;
            var response = await section.GetAsync(endpoint);

            //Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var results = await response.Content.ReadFromJsonAsync<UserVote[]>();

            results.Should().BeInAscendingOrder(uv => uv.Value);
            results.All(uv => uv.Post != null).Should().BeTrue();
            results.Should().HaveCount(pageSize);


        }

        [Fact]
        public async Task Vote_ExistingPost_ShouldReturn201()
        {
            //Arrange
            var loggedUser = await _userFixture.GetOrCreate(_web);
            var http = _userFixture.Sections[loggedUser];
            var endPoint = EndPoints.Resolve<UserVotesController>(UserVotesController.ActionVote);

            //Preenchendo entidades dependentes
            var userFaker = new UserFaker();
            var postFazer = new PostFaker();
            var otherUser = await _userFixture.Create(userFaker.Generate(), "outroUser123", _web);
            var otherUserSection = _userFixture.Sections[otherUser];

            var newPost = await QuickCreate.PostPost(otherUserSection, postFazer.Generate("default"));


            //Act
            var dto = new UserVoteRequestDto
            {
                PostId = newPost.Id,
                Value = 8
            };
            var response = await http.PutAsJsonAsync(endPoint, dto);

            //Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        }
        [Fact]
        public async Task Vote_NotExistingPost_ShouldReturn404Or422()
        {
            //Arrange
            var loggedUser = await _userFixture.GetOrCreate(_web);
            var http = _userFixture.Sections[loggedUser];
            var endPoint = EndPoints.Resolve<UserVotesController>(UserVotesController.ActionVote);

            //Preenchendo entidades dependentes
            var userFaker = new UserFaker();

            //Act
            var response = await http.PutAsJsonAsync(endPoint, new UserVoteRequestDto
            {
                PostId = int.MinValue,
                Value = 8
            });

            //Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().BeOneOf(HttpStatusCode.UnprocessableEntity, HttpStatusCode.NotFound);

        }
        [Fact]
        public async Task Unvote_ExistingPost_ShouldReturn200()
        {
            //Arrange
            var loggedUser = await _userFixture.GetOrCreate(_web);
            var http = _userFixture.Sections[loggedUser];
            var voteEndpoint = EndPoints.Resolve<UserVotesController>(UserVotesController.ActionVote);
            

            //Preenchendo entidades dependentes
            var userFaker = new UserFaker();
            var postFazer = new PostFaker();
            var otherUser = await _userFixture.Create(userFaker.Generate(), "outroUser123", _web);
            var otherUserSection = _userFixture.Sections[otherUser];

            var newPost = await QuickCreate.PostPost(otherUserSection, postFazer.Generate("default"));
            var newVote = await QuickCreate.PutVote(http, new UserVoteRequestDto
            {
                PostId = newPost.Id,
                Value = 10
            });

            //Act
            var unvoteEndPoint = EndPoints.Resolve<UserVotesController>(UserVotesController.ActionUnvote.Placeholder(newPost.Id.ToString()));
            var response = await http.DeleteAsync(unvoteEndPoint);

            //Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        }

        public Task DisposeAsync()
            => Task.CompletedTask;

        public async Task InitializeAsync()
        {
            _imagesSet.ReadPath();
            await _imagesSet.TryPopulate(10);
        }
    }
}
