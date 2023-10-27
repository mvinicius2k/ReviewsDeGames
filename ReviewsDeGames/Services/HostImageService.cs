using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.AspNetCore.WebUtilities;
using ReviewsDeGames.Helpers;
using ReviewsDeGames.Models;

namespace ReviewsDeGames.Services
{
    public interface IHostImageService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public Task<string> Write(byte[] image, string filename, string userId, DateOnly date);
        /// <summary>
        /// Transforma <paramref name="filename"/> em um nome único
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public string UniqueName(string filename);
        public void Delete(string filename, string userId, DateOnly date);

    }

  

    public class HostImageService : IHostImageService
    {
        public const char UniqueNameSeparator = '-';
        private readonly IWebHostEnvironment _environment;

        public HostImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }


        public string UniqueName(string filename)
        {
            var guid = Guid.NewGuid();
            var guid64 = guid.ToSafeBase64();
            return Path.GetFileNameWithoutExtension(filename) + UniqueNameSeparator + guid64 + Path.GetExtension(filename);
        }

        public async Task<string> Write(byte[] image, string filename, string userId, DateOnly date)
        {
            var folder = GetImageFolder(userId, date);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// /// <exception cref="System.Security.SecurityException"></exception>
        public void Delete(string filename, string userId, DateOnly date)
        {
            var folder = GetImageFolder(userId, date);
            var fullPath = Path.Combine(folder, filename);
            var fileInfo = new FileInfo(fullPath);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            else 
                throw new FileNotFoundException();


        }

        public string GetImageFolder(string userId, DateOnly dt)
        {
            return Path.Combine(_environment.WebRootPath, Values.UserImagesFolderName, userId, $"{dt.Year}-{dt.Month}");
        }
    }
}
