using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Moq;
using ReviewsDeGames.Services;
using ReviewsGamesIntegrationTests.Fakers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewsGamesTests.Unit
{
    public class ImageTest
    {
        private readonly IHostImageService _hostImage;

        public ImageTest()
        {
            var hostEnvMock = new Mock<IWebHostEnvironment>();
            hostEnvMock.Setup(d => d.WebRootPath).Returns(Directory.GetCurrentDirectory());
            _hostImage = new HostImageService(hostEnvMock.Object);
        }

        [Fact]
        public async Task Delete_OwnUpload_ShouldSucess()
        {
            //Arrange
            var faker = new Faker();
            var http = new HttpClient();
            var reponse = await http.GetAsync(faker.Image.PicsumUrl());
            var imageBytes = await reponse.Content.ReadAsByteArrayAsync();
            var userId = faker.Random.Guid().ToString();
            var filename = faker.Lorem.Word() + ".jpg";
            filename = _hostImage.UniqueName(filename);
            var dt = DateOnly.FromDateTime(DateTime.UtcNow);

            //act-1
            var fullPath = await _hostImage.Write(imageBytes, filename, userId, dt);

            //assert-1
            File.Exists(fullPath).Should().BeTrue();


            //act-2
            _hostImage.Delete(filename, userId, dt);

            //assert-2
            File.Exists(fullPath).Should().BeFalse();

        }

        [Theory, MemberData(nameof(ValidImagesParams))]
        public async Task Send_ValidImages_ShouldSucess(byte[] image, string filename, string userId)
        {

            filename = _hostImage.UniqueName(filename);
            var result =  await _hostImage.Write(image, filename, userId, DateOnly.FromDateTime(DateTime.UtcNow));


            result.Should().NotBeNullOrWhiteSpace();

            var savedFilename = Path.GetFileName(result);

            //Verificando nome do arquivo e extensão
            savedFilename.Should()
                .StartWith(Path.GetFileNameWithoutExtension(filename)).And
                .EndWith(Path.GetExtension(filename));

            File.Exists(result).Should().BeTrue(); //O arquivo existe

        }

       

        #region MemberData
        public static IEnumerable<object[]> ValidImagesParams()
        {
            List<object[]> data = ImageBytes();

            foreach (var item in data)
                yield return item;
        }

        private static List<object[]> ImageBytes()
        {
            var count = 6;
            var data = new List<object[]>(count);
            var faker = new Faker();
            var http = new HttpClient();
            var users = new string[] { faker.Random.Guid().ToString(), faker.Random.Guid().ToString() }; //2 users

            for (int i = 0; i < count; i++)
            {
                var randomId = faker.PickRandom(users);
                var randomImageUrl = faker.Image.PicsumUrl();
                var response = http.GetAsync(randomImageUrl).Result; // <- 
                var bytes = response.Content.ReadAsByteArrayAsync().Result; // <-
                data.Add(new object[] { bytes, faker.Lorem.Word() + ".jpg", randomId });

            }

            return data;
        }

        #endregion

    }
}
