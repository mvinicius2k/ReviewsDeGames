using Bogus;
using Bogus.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewsGamesTests.Helpers
{
    public static class WebUtils
    {
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
