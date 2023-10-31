using ReviewsDeGames.Controllers;
using ReviewsGamesIntegrationTests.Helpers;
using ReviewsGamesTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ReviewsGamesTests.Helpers
{
    public class ImagePopulate : IPopulate
    {
        public string FolderName { get; private set; } = TestValues.ImagesFolderName;

        

        public async Task<HttpResponseMessage> Populate(HttpClient http)
        {
            var imageSet = new ImagesSet
            {
                RelativeFolderPath = FolderName
            };
            imageSet.ReadPath();


            var endpoint = EndPoints.Resolve<ImagesController>(ImagesController.ActionAdd);
            var multipart = await http.PostMultipartFiles(imageSet.Images.ToArray(), endpoint, ImagesController.AddFilesForm);

            return multipart;
        }
    }
}
