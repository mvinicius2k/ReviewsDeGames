using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MimeMapping;
using Newtonsoft.Json;
using ReviewsDeGames.Controllers;
using ReviewsDeGames.Models;
using ReviewsGamesIntegrationTests.Fakers;
using ReviewsGamesIntegrationTests.Fixtures;
using ReviewsGamesIntegrationTests.Helpers;
using ReviewsGamesTests.Fixtures;
using ReviewsGamesTests.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ReviewsGamesTests.Integration
{
    public class ImageTest : IClassFixture<WebFactory>, IClassFixture<UserFixture>, IClassFixture<ImagesSetFixture>
    {
        private readonly WebFactory _web;
        private readonly UserFixture _userFixture;
        private readonly ImagesSetFixture _imagesSet;

        public ImageTest(WebFactory web, UserFixture userFixture, ImagesSetFixture imagesSet)
        {
            _web = web;
            _userFixture = userFixture;
            _imagesSet = imagesSet;
        }

        [Fact]
        public async Task Add_JpgImage_ShouldReturn201()
        {
            //Arrange
            var count = 6;
            var faker = new Faker();
            var loggedUser = await _userFixture.GetOrCreate(_web);
            var endpoint = EndPoints.Resolve<ImagesController>(ImagesController.ActionAdd);
            var http = _userFixture.Sections[loggedUser];
            if (!_imagesSet.Images.Any())
                _imagesSet.ReadPath();
            await _imagesSet.TryPopulate(count);
            var images = faker.PickRandom(_imagesSet.Images.ToArray(), count);

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            using (var content = new MultipartFormDataContent())
            {
                foreach (var image in images)
                {
                    var stream = File.OpenRead(image.FullName);
                    content.Add(new StreamContent(stream), "files", image.Name);
                }
                request.Content = content;
                var response = await http.SendAsync(request);
                response.StatusCode.Should().Be(System.Net.HttpStatusCode.MultiStatus);
                var multiResponse = await response.Content.ReadFromJsonAsync<IEnumerable<MultiStatusResponse<ImageResponseDto>>>();
                multiResponse.Should().NotBeNullOrEmpty();
                Assert.All(multiResponse, (res) => res.StatusCode.Should().Be(StatusCodes.Status201Created));

            }




        }
        //[Theory]
        //public async Task Add_InvalidImage_ShouldFail(ImageUploadDto dto)
        //{

        //}

        [Fact]
        public async Task Delete_OwnImage_ShouldReturn200()
        {

        }

        #region MemberData

        #endregion
    }
}
