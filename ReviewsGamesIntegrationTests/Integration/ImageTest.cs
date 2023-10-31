using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MimeMapping;
using Newtonsoft.Json;
using ReviewsDeGames.Controllers;
using ReviewsDeGames.Helpers;
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
    public class ImageTest : IClassFixture<WebFactory>, IClassFixture<UserFixture>, IClassFixture<ImagesSet>
    {
        private readonly WebFactory _web;
        private readonly UserFixture _userFixture;
        private readonly ImagesSet _imagesSet;

        public ImageTest(WebFactory web, UserFixture userFixture, ImagesSet imagesSet)
        {
            _web = web;
            _userFixture = userFixture;
            _imagesSet = imagesSet;

            _imagesSet.ReadPath();
        }

        [Fact]
        public async Task Add_JpgImages_ShouldAllSucess()
        {
            //Arrange
            var count = 6;
            var faker = new Faker();
            var loggedUser = await _userFixture.GetOrCreate(_web);
            var endpoint = EndPoints.Resolve<ImagesController>(ImagesController.ActionAdd);
            var http = _userFixture.Sections[loggedUser];

            await _imagesSet.TryPopulate(count);
            var images = faker.PickRandom(_imagesSet.Images.ToArray(), count).ToArray();


            //Act
            var response = await http.PostMultipartFiles(images, endpoint);

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.MultiStatus);
            var multiResponse = await response.Content.ReadFromJsonAsync<IEnumerable<MultiStatusResponse<ImageResponseDto>>>();
            multiResponse.Should().NotBeNullOrEmpty();
            Assert.All(multiResponse, (res) => res.StatusCode.Should().Be(StatusCodes.Status201Created));

        }





        private FileInfo? CreateRandomTempTxt()
        {
            try
            {
                var path = Path.Combine(Path.GetTempPath(), Path.GetTempFileName() + ".txt");
                var faker = new Faker();
                using (var stream = File.CreateText(path))
                    stream.WriteLine(faker.Lorem.Paragraph());
                return new FileInfo(path);

            }
            catch (Exception)
            {
                return null;

            }
        }

        [Fact]
        public async Task Add_SomeInvalidImages_ShouldSomeFail()
        {
            //Arrange
            var validCount = 2;
            var invalidCount = 2;
            var faker = new Faker();
            var loggedUser = await _userFixture.GetOrCreate(_web);
            var endpoint = EndPoints.Resolve<ImagesController>(ImagesController.ActionAdd);
            var http = _userFixture.Sections[loggedUser];
            await _imagesSet.TryPopulate(validCount + invalidCount);
            var validImages = faker.PickRandom(_imagesSet.Images.ToArray(), validCount).ToArray();
            var invalidImages = new FileInfo[invalidCount];
            for (int i = 0; i < invalidImages.Length; i++)
                invalidImages[i] = CreateRandomTempTxt();

            var images = faker.Random.Shuffle(validImages.Concat(invalidImages)).ToArray();

            //Act
            var response = await http.PostMultipartFiles(images, endpoint);

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.MultiStatus);
            var multiResponse = await response.Content.ReadFromJsonAsync<IEnumerable<MultiStatusResponse<ImageResponseDto>>>();
            multiResponse.Should().NotBeNullOrEmpty();
            Assert.All(multiResponse, (res) =>
            {
                //Para os códigos de sucesso, as imagem devem ser do conjunto de válidas
                if (res.StatusCode == StatusCodes.Status201Created)
                {
                    var solvedFilename = string.Join("-", res.Result.FileName.Split("-").SkipLast(1)) + Path.GetExtension(res.Result.FileName);
                    solvedFilename.Should().BeOneOf(validImages.Select(x => x.Name));
                    //var filename = validImages.Select(info => info.Name).First(name => res.Result.FileName.StartsWith(Path.GetFileNameWithoutExtension(name)));
                    //res.Result.FileName.Should().StartWith(Path.GetFileNameWithoutExtension(filename));
                    

                }
                //Para os códigos de falha, as imagem devem ser do conjunto de inválidas e informarem o nome do arquivo
                else
                {
                    res.StatusCode.Should().BeOneOf(StatusCodes.Status422UnprocessableEntity, StatusCodes.Status400BadRequest);
                    var invalidFileNames = invalidImages.Select(info => info.Name);
                    res.Message.Should().ContainAny(invalidFileNames);
                }
            });

        }
        [Fact]
        public async Task Delete_OwnImage_ShouldReturn200()
        {
            //Arrange
            var faker = new Faker();
            var loggedUser = await _userFixture.GetOrCreate(_web);
            var addEndpoint = EndPoints.Resolve<ImagesController>(ImagesController.ActionAdd);
            var http = _userFixture.Sections[loggedUser];

            await _imagesSet.TryPopulate(1);
            var images = faker.PickRandom(_imagesSet.Images.ToArray(), 1).ToArray();


            //Act
            var response = await http.PostMultipartFiles(images, addEndpoint);

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.MultiStatus);
            var multiResponse = await response.Content.ReadFromJsonAsync<IEnumerable<MultiStatusResponse<ImageResponseDto>>>();
            multiResponse.Should().NotBeNullOrEmpty();
            var id = multiResponse.First().Result.Id;

            var removeEndpoint = EndPoints.Resolve<ImagesController>(ImagesController.ActionDelete.Placeholder(id.ToString()));
            var deleteResponse = await http.DeleteAsync(removeEndpoint);
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            
        }
    }


    #region MemberData

    #endregion
}

