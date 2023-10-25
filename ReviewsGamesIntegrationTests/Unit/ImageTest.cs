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
        [Theory, MemberData(nameof(ValidImagesParams))]
        public async Task Send_ValidImages_ShouldSucess(byte[] image, string filename, string userId)
        {
            var hostEnvMock = new Mock<IWebHostEnvironment>();
            hostEnvMock.Setup(d => d.WebRootPath).Returns(Directory.GetCurrentDirectory());

            var hostImage = new HostImageService(hostEnvMock.Object);

            var result =  await hostImage.Send(image, filename, userId);


            result.Should().NotBeNullOrWhiteSpace();

            var savedFilename = Path.GetFileName(result);

            //Verificando nome do arquivo e extensão
            savedFilename.Should()
                .StartWith(Path.GetFileNameWithoutExtension(filename)).And
                .EndWith(Path.GetExtension(filename));
            //Verificando se o id único foi inserido
            Path.GetFileNameWithoutExtension(savedFilename.Split('-').Last()).Should().NotBeNullOrWhiteSpace();

            File.Exists(result).Should().BeTrue(); //O arquivo existe

        }

        #region MemberData
        public static IEnumerable<object[]> ValidImagesParams()
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

            foreach (var item in data)
                yield return item;
        }

        #endregion

    }
}
