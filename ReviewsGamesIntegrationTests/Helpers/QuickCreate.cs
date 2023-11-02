using Microsoft.AspNetCore.Http;
using ReviewsDeGames.Controllers;
using ReviewsDeGames.Models;
using ReviewsGamesIntegrationTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ReviewsGamesTests.Helpers
{
    
    public static class QuickCreate
    {
        public static async Task<ImageResponseDto[]> PostImage(HttpClient http, FileInfo[] files)
        {
            var imageResponse = await http.PostMultipartFiles(
                            files: files,
                            endPoint: EndPoints.Resolve<ImagesController>(ImagesController.ActionAdd), ImagesController.AddFilesForm);
            if (!imageResponse.IsSuccessStatusCode)
                Assert.Fail("Não foi possível enviar imagem");
            var imageDto = await imageResponse.Content.ReadFromJsonAsync<MultiStatusResponse<ImageResponseDto>[]>();
            if(imageDto == null)
            {
                Assert.Fail("Deserialização da resposta com erro");
                
            }
            var result=  imageDto
                .Where(r => r.StatusCode == StatusCodes.Status201Created)
                .Select(m => m.Result!).ToArray();
            return result;
        }
    }
}
