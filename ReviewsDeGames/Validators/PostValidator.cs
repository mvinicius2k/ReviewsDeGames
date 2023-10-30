using FluentValidation;
using ReviewsDeGames.Helpers;
using ReviewsDeGames.Models;
using ReviewsDeGames.Services;

namespace ReviewsDeGames.Validators
{
    public class PostValidator : AbstractValidator<PostRequestDto>
    {
        public PostValidator(IDescribesService describes)
        {
            RuleFor(p => p.Title)
                .NotEmpty()
                .MaximumLength(Post.TitleMaxLength)
                .WithMessage(describes.NotEmptyOrMaxLength(Post.TitleMaxLength));
            RuleFor(p => p.Text)
                .NotEmpty()
                .WithMessage(describes.NotEmpty());



            RuleFor(p => p.FeaturedImageUrl)
                .SupportedImageUrl(Values.SupportedImageExtensions).When(p => !string.IsNullOrWhiteSpace(p.FeaturedImageUrl))
                .WithMessage(describes.FormatNotSupported(Values.SupportedImageExtensions));

        }
    }
}
