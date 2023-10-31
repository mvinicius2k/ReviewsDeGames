using Bogus;
using FluentAssertions;
using ReviewsDeGames.Controllers;
using ReviewsDeGames.Models;
using ReviewsGamesIntegrationTests.Fakers;
using ReviewsGamesIntegrationTests.Fixtures;
using ReviewsGamesIntegrationTests.Helpers;
using ReviewsGamesTests.Fakers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ReviewsGamesTests.Integration
{
    public class PostsTest : IClassFixture<WebFactory>, IClassFixture<UserFixture>
    {
        private readonly WebFactory _web;
        private readonly UserFixture _userFixture;


        public PostsTest(WebFactory web, UserFixture userFixture)
        {
            _web = web;
            _userFixture = userFixture;
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



    }
}
