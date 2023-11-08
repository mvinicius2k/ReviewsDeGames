using FluentValidation;
using ReviewsDeGames.Models;
using ReviewsDeGames.Repository;
using ReviewsDeGames.Services;

namespace ReviewsDeGames.Validators
{
    public class UserVoteValidator : AbstractValidator<UserVoteRequestDto>
    {
        /// <summary>
        /// Verifica se o post existe
        /// </summary>
        public UserVoteValidator(IDescribesService describes, IPostRepository posts)
        {
            RuleFor(uv => uv.PostId)
                .MustAsync(async (postId, token) =>
                {
                    var post = await posts.GetById(postId);
                    return post != null;

                }).WithMessage(p => describes.EntityNotFound(nameof(Post), p.PostId));
        }
    }
}
