﻿using Microsoft.Data.SqlClient;
using MimeMapping;

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


        public readonly static string[] SupportedMime = new string[] {
            MimeMapping.KnownMimeTypes.Jpg,
            MimeMapping.KnownMimeTypes.Png,
            MimeMapping.KnownMimeTypes.Jpeg,
            MimeMapping.KnownMimeTypes.Gif,
            MimeMapping.KnownMimeTypes.Tiff,
            MimeMapping.KnownMimeTypes.Webp,

        };
        public static readonly string[] SupportedExtensions = SupportedMime
            .Select(m => MimeUtility.GetExtensions(m))
            .SelectMany(i => i.Select(ex => "." + ex))
            .ToArray();

        public const string RoleAdmin = "admin";





    }


}
