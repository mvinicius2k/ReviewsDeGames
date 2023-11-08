using Bogus;
using Bogus.DataSets;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using ReviewsDeGames.Controllers;
using ReviewsDeGames.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ReviewsGamesTests.Helpers
{
    public static class HttpUtils
    {
        public static async Task<HttpResponseMessage> PostMultipartFiles(this HttpClient http, IEnumerable<FileInfo> files, string endPoint, string fieldName = "files")
        {
            
            using var request = new HttpRequestMessage(HttpMethod.Post, endPoint);
            using (var content = new MultipartFormDataContent())
            {
                foreach (var image in files)
                {
                    var stream = File.OpenRead(image.FullName);
                    content.Add(new StreamContent(stream), fieldName, image.Name);
                }
                request.Content = content;
                return await http.SendAsync(request);

            }
        }

        public static async Task<byte[]?> ImageBytes(string url)
        {
            var http = new HttpClient();
            var response = await http.GetAsync(url);
            if(!response.IsSuccessStatusCode)
                return null;
            var bytes = await response.Content.ReadAsByteArrayAsync();
            return bytes;


        }

        public static async Task<bool> TryDownloadImage(string url, string pathSave)
        {
            var bytes = await ImageBytes(url);
            if(bytes == null)
                return false;
            await File.WriteAllBytesAsync(pathSave, bytes);
            return true;
        }
    }
}
