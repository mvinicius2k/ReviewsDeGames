using Microsoft.AspNetCore.WebUtilities;
using ReviewsDeGames.Helpers;

namespace ReviewsDeGames.Services
{
    public interface IHostImageService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="filenamePrefix"></param>
        /// <returns></returns>
        public Task<string> Send(byte[] image, string filename, string userId);


        
    }

    public class HostImageService : IHostImageService
    {
        private readonly IWebHostEnvironment _environment;

        public HostImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        private string InsertGuid64(string filename, char separator = '-')
        {
            var guid = Guid.NewGuid();
            var guid64 = Convert.ToBase64String(guid.ToByteArray()).Substring(0, 22)
                .Replace("+","_")
                .Replace("/",",")
                .Replace("=","");
            return Path.GetFileNameWithoutExtension(filename) + separator + guid64 + Path.GetExtension(filename);
        }

        public async Task<string> Send(byte[] image, string filename, string userId)
        {
            var folder = GetImageFolder(userId, DateOnly.FromDateTime(DateTime.UtcNow));
            filename = InsertGuid64(filename);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);


            var fullpath = Path.Combine(folder, filename);
            var byteArr = new BinaryData(image).ToStream();
            using (var stream = new FileStream(fullpath, FileMode.Create))
            {
                await byteArr.CopyToAsync(stream);
            }

            return fullpath;
        }

        public string GetImageFolder(string userId, DateOnly dt)
        {
            return Path.Combine(_environment.WebRootPath, Values.UserImagesFolderName, userId, $"{dt.Year}-{dt.Month}");
        }
    }
}
