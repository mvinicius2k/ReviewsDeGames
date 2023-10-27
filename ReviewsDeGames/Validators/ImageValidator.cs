using FluentValidation;
using MimeMapping;
using ReviewsDeGames.Models;
using ReviewsDeGames.Services;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace ReviewsDeGames.Validators
{
    public class ImageValidator : AbstractValidator<ImageRequestDto>
    {
        public const int MaxLengthBytes = 1024 * 1024 * 8; //8MB 


        public static string[] SupportedMime = new string[] {
            MimeMapping.KnownMimeTypes.Jpg,
            MimeMapping.KnownMimeTypes.Png,
            MimeMapping.KnownMimeTypes.Jpeg,
            MimeMapping.KnownMimeTypes.Gif,
            MimeMapping.KnownMimeTypes.Tiff,
            MimeMapping.KnownMimeTypes.Webp,

        };

        public ImageValidator(IDescribesService describes)
        {
            RuleFor(i => i.FormFile)
                .Must((i) =>
                {
                    var extensionMime = MimeUtility.GetMimeMapping(i.FileName);
                    return SupportedMime.Contains(extensionMime);
                })
                .WithMessage(i => describes.ContentTypeNotSupported(i.FormFile.ContentType))
                .Must(i => i.Length <= MaxLengthBytes)
                .WithMessage(describes.MaxSizeOverflowMB(MaxLengthBytes));

        }
    }
}
