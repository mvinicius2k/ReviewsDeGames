using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewsGamesTests.Helpers
{

    public class TestValues
    {
        public const string ImagesFolderName = "images";
        public const string AvatarsFolderName = "avatars";

        public static string ImagesPath => Path.Combine(Directory.GetCurrentDirectory(), ImagesFolderName);
    }
}
