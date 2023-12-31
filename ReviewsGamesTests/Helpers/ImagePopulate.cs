﻿using ReviewsDeGames.Controllers;
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
    /// <summary>
    /// Preenche e envia para o servidor imagens de um <see cref="ImagesSet"/>
    /// </summary>
    public class ImagePopulate : IPopulate
    {
        public ImagesSet ImagesSet { get; private set; }

        public ImagePopulate(ImagesSet imagesSet)
        {
            ImagesSet = imagesSet;
        }

        public async Task<HttpResponseMessage> Populate(HttpClient http)
        {

            ImagesSet.ReadPath();


            var endpoint = EndPoints.Resolve<ImagesController>(ImagesController.ActionAdd);
            var multipart = await http.PostMultipartFiles(ImagesSet.Images.ToArray(), endpoint, ImagesController.AddFilesForm);

            return multipart;
        }
    }
}
