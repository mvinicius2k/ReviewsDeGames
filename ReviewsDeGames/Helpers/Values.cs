using Microsoft.Data.SqlClient;

namespace ReviewsDeGames.Helpers
{
    public static class Values
    {
        public const string ODataPrefixRoute = "odata";
        public const int ODataMaxTop = 20;

        public const string SqlConnection = "SQLServerConnection";
        public const string SqlConnectionForTests = "SQLServerTests";

        public const int MaxImageUrlLength = 320;
        public const int IdentityIdMaxLength = 450;

        public const string UserImagesFolderName = "userImages";

        
        public static readonly string[] SupportedImageExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg" };

        public const string RoleAdmin = "admin";





    }


}
