using FluentValidation;
using MimeMapping;
using ReviewsDeGames.Controllers;
using ReviewsDeGames.Helpers;
using ReviewsDeGames.Models;
using ReviewsDeGames.Services;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace ReviewsDeGames.Validators
{
    public class ImageValidator : AbstractValidator<ImageRequestDto>
    {
        public const int MaxLengthBytes = 1024 * 1024 * 8; //8MB 
        public static int MaxLenghtFilename  = Image.FilenameMaxLength - new Guid().ToString().Length;

        public ImageValidator(IDescribesService describes)
        {
            RuleFor(i => i.FormFile)
                .Must((i) =>
                {
                    return Values.SupportedExtensions.Contains(Path.GetExtension(i.FileName));
                })
                .WithMessage(i => describes.FormatNotSupported(Values.SupportedExtensions))
                .Must(i => i.Length <= MaxLengthBytes)
                .WithMessage(describes.MaxSizeOverflowMB(MaxLengthBytes))
                .Must(i => i.FileName.Length <= MaxLenghtFilename)
                .WithMessage(describes.FileNameMaxLength(MaxLenghtFilename))
                ;
            

        }
    }
}
