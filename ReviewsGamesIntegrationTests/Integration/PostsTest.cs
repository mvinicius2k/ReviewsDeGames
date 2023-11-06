using Bogus;
using FluentAssertions;
using MimeMapping;
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
using System.Text.Json;
using System.Threading.Tasks;

namespace ReviewsGamesTests.Integration
{
    public class PostsTest : IClassFixture<WebFactory>, IClassFixture<UserFixture>, IClassFixture<ImagesSet>, IAsyncLifetime
    {
        private readonly WebFactory _web;
        private readonly UserFixture _userFixture;
        private readonly ImagesSet _imagesSet;

        public PostsTest(WebFactory web, UserFixture userFixture, ImagesSet imagesSet)
        {
            _web = web;
            _userFixture = userFixture;
            _imagesSet = imagesSet;
        }

        [Theory, MemberData(nameof(ValidPostsParams))]
        public async Task Create_ValidPosts_ShouldReturn201(PostRequestDto postDto)
        {
            var loggedUser = await _userFixture.GetOrCreate(_web);
            var http = _userFixture.Sections[loggedUser];
            var endPOint = EndPoints.Resolve<PostsController>(PostsController.ActionCreate);
            //Act
            var response = await http.PostAsJsonAsync(endPOint, postDto);

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        }
        [Theory, MemberData(nameof(InvalidPostsParams))]
        public async Task Create_InvalidPosts_ShouldReturn400Or422(PostRequestDto postDto)
        {
            var loggedUser = await _userFixture.GetOrCreate(_web);
            var http = _userFixture.Sections[loggedUser];
            var endPOint = EndPoints.Resolve<PostsController>(PostsController.ActionCreate);
            
            //Act
            var response = await http.PostAsJsonAsync(endPOint, postDto);

            //Assert
            response.StatusCode.Should().BeOneOf(System.Net.HttpStatusCode.UnprocessableEntity, HttpStatusCode.BadRequest);

        }

        [Fact]
        public async Task Edit_ValidPost_ShouldReturn200()
        {
            //Arrange
            var faker = new Faker();
            var loggedUser = await _userFixture.GetOrCreate(_web);
            var http = _userFixture.Sections[loggedUser];
            var postFaker = new PostFaker();

            var postFake = postFaker.Generate("default");
            //Obtendo imagem do post
            await _imagesSet.TryPopulate(1);
            var randomImage = faker.PickRandom(_imagesSet.Images.ToArray());
            var image = await QuickCreate.PostImage(http, new FileInfo[] { randomImage });
            var imageId = image.First().Id;


            //Inserindo chave estrangeira
            postFake = postFake with { FeaturedImageUrl = imageId };

            //enviando e obtendo um post para editar
            var postToEdit = await QuickCreate.PostPost(http, postFake);
            var endPoint = EndPoints.Resolve<PostsController>(PostsController.ActionEdit).Placeholder(postToEdit.Id.ToString());

            //Act
            var newPost = new PostRequestDto
            {
                Title = " Um título realmente não randômico",
                Text = "Um texto aqui",
                FeaturedImageUrl = null
            };
            var content = new StringContent(JsonSerializer.Serialize(newPost), Encoding.UTF8, KnownMimeTypes.Json);
            var response = await http.PatchAsync(endPoint, content);

            //Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK); 
            var serialized = await response.Content.ReadFromJsonAsync<Post>();
            serialized.Should().NotBeNull();
        }

        [Fact]
        public async Task Edit_InvalidPost_ShouldReturn422()
        {
            //Arrange
            var loggedUser = await _userFixture.GetOrCreate(_web);
            var http = _userFixture.Sections[loggedUser];
            var postFaker = new PostFaker();

            var postFake = postFaker.Generate("default");

            //enviando e obtendo um post para editar
            var postToEdit = await QuickCreate.PostPost(http, postFake);
            var endPoint = EndPoints.Resolve<PostsController>(PostsController.ActionEdit).Placeholder(postToEdit.Id.ToString());

            //Act
            var rulesetBad = string.Join(",", "default", PostFaker.BadTitleRule, PostFaker.BadFeaturedImageRule);
            var newPost = postFaker.Generate(rulesetBad);
            var content = new StringContent(JsonSerializer.Serialize(newPost), Encoding.UTF8, KnownMimeTypes.Json);
            var response = await http.PatchAsync(endPoint, content);

            //Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            
        }
        [Fact]
        public async Task Delete_OwnPost_ShouldReturn200()
        {
            //Arrange
            var loggedUser = await _userFixture.GetOrCreate(_web);
            var http = _userFixture.Sections[loggedUser];
            var postFaker = new PostFaker();

            var postFake = postFaker.Generate("default");

            //enviando e obtendo um post para deletar
            var postToEdit = await QuickCreate.PostPost(http, postFake);
            var endPoint = EndPoints.Resolve<PostsController>(PostsController.ActionDelete).Placeholder(postToEdit.Id.ToString());

            //Act
            var response = await http.DeleteAsync(endPoint);

            //Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        #region MemberData
        public static IEnumerable<object[]> ValidPostsParams()
        {
            var count = 50;
            var faker = new PostFaker();
            var posts = faker.Generate(count, "default");

            foreach (var post in posts)
                yield return new object[] { post };


        }
        public static IEnumerable<object[]> InvalidPostsParams()
        {
            var count = 50;
            var badTitleRule = $"default, {PostFaker.BadTitleRule}"; 
            var badImageRule = $"default, {PostFaker.BadFeaturedImageRule}";
            var faker = new PostFaker();
            var badTitle = faker.Generate(count/2, badTitleRule);
            var badImage = faker.Generate(count/2, badImageRule);
            var posts = new Faker().Random.Shuffle(badTitle.Concat(badImage)).ToList();

            foreach (var post in posts)
                yield return new object[] { post };


        }

        #endregion

        public async Task InitializeAsync()
        {
            _imagesSet.ReadPath();
            await _imagesSet.TryPopulate(10);
        }

        public Task DisposeAsync()
         =>
            Task.CompletedTask;


    }
}
