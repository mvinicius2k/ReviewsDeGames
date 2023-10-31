using FluentValidation;
using ReviewsDeGames.Helpers;
using ReviewsDeGames.Models;
using ReviewsDeGames.Repository;
using ReviewsDeGames.Services;

namespace ReviewsDeGames.Validators
{
    public class PostValidator : AbstractValidator<PostRequestDto>
    {
        public PostValidator(IDescribesService describes, IImagesRepository images)
        {
            RuleFor(p => p.Title)
                .NotEmpty()
                .MaximumLength(Post.TitleMaxLength)
                .WithMessage(describes.NotEmptyOrMaxLength(Post.TitleMaxLength));
            RuleFor(p => p.Text)
                .NotEmpty()
                .WithMessage(describes.NotEmpty());



            RuleFor(u => u.FeaturedImageUrl)
                 .MustAsync(async (avtId, _) => (await images.GetById(avtId.Value)) != null).When(post => post.FeaturedImageUrl != null)
                 .WithMessage(u => describes.EntityNotFound(nameof(Image), u.FeaturedImageUrl));

        }
    }
}
