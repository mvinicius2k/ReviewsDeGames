using Microsoft.AspNetCore.Http;
using ReviewsDeGames.Controllers;
using ReviewsDeGames.Models;
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
    /// Cria entidades randomicamente ou causa um Assert.Fail em caso de erro
    /// </summary>
    public static class QuickCreate
    {
        public static async Task<Post> PostPost(HttpClient http, PostRequestDto dto)
        {
            var endpoint = EndPoints.Resolve<PostsController>(PostsController.ActionCreate);
            var response = await http.PostAsJsonAsync(endpoint, dto);
            if (response == null || !response.IsSuccessStatusCode)
                Assert.Fail("Não foi possível criar um post");
            var post = await response.Content.ReadFromJsonAsync<Post>();
            if (post == null)
                Assert.Fail("Não foi desserializar a resposta de post");
            return post;
        }
        
        public static async Task<ImageResponseDto[]> PostImage(HttpClient http, FileInfo[] files)
        {
            var imageResponse = await http.PostMultipartFiles(
                            files: files,
                            endPoint: EndPoints.Resolve<ImagesController>(ImagesController.ActionAdd), ImagesController.AddFilesForm);
            if (!imageResponse.IsSuccessStatusCode)
                Assert.Fail("Não foi possível enviar imagem");
            var imageDto = await imageResponse.Content.ReadFromJsonAsync<MultiStatusResponse<ImageResponseDto>[]>();
            if (imageDto == null)
            {
                Assert.Fail("Deserialização da resposta com erro");

            }
            var result = imageDto
                .Where(r => r.StatusCode == StatusCodes.Status201Created)
                .Select(m => m.Result!).ToArray();
            return result;
        }
    }
}
