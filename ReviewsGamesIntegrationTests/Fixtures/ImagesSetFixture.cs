using Bogus;
using Microsoft.VisualBasic;
using ReviewsDeGames.Helpers;
using ReviewsDeGames.Validators;
using ReviewsGamesTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewsGamesTests.Fixtures
{
    public class ImagesSetFixture
    {
        public readonly static string[] SupportedTypes = new string[] { ".jpg", ".jpeg", ".png", "bmp" };
        public int Width { get; set; } = 64;
        public int Height { get; set; } = 64;
        public string RelativeFolderPath { get; set; } = "testImages";
        public HashSet<FileInfo> Images { get; private set; } = new();
        public string ImageFolder => Path.Combine(Directory.GetCurrentDirectory(), RelativeFolderPath);
        
        private void ResolveDirectory()
        {
            if (!Directory.Exists(ImageFolder))
                Directory.CreateDirectory(ImageFolder);
        }
        public void ReadPath()
        {
            ResolveDirectory();
            var files = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), RelativeFolderPath));
            var imagesName = files.Where(f => SupportedTypes.Contains(Path.GetExtension(f)))
                .Select(i => new FileInfo(i));
            for (int i = 0; i < imagesName.Count(); i++)
                Images.Add(imagesName.ElementAt(i));
        }
        /// <summary>
        /// Tenta preencher o restante do conjunto de arquivos de imagem
        /// </summary>
        /// <returns><see langword="true"/> se for TOTALMENTE preechido, <see langword="false"/> caso contrário</returns>
        public async Task<bool> TryPopulate(int count, int maxNetworkAttempts = 10, int delayBetweenAttemptsMs = 500)
        {
            if (Images.Count >= count)
                return false;
            
            var faker = new Faker();
                ResolveDirectory();

            var attemps = maxNetworkAttempts;
            while(Images.Count < count)
            {
                var imageUrl = faker.Image.PicsumUrl(Width, Height);
                var savePath = Path.Combine(ImageFolder, faker.Random.Guid().ToSafeBase64() + ".jpg");
                var result = await WebUtils.TryDownloadImage(imageUrl, savePath);
                if (result)
                {
                    attemps = 0;
                    Images.Add(new FileInfo(savePath));
                }
                else
                {
                    attemps++;
                    Thread.Sleep(delayBetweenAttemptsMs);
                }
                if (attemps > maxNetworkAttempts)
                    return false;
            }

            return true;
        }


    }
}
