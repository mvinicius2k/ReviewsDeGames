using ReviewsDeGames.Helpers;
using System.ComponentModel.DataAnnotations;

namespace ReviewsDeGames.Models
{
    public class Post : IModel<int>
    {
        public const int TitleMaxLength = 100;
        public const int FeaturedImageMaxLength = Values.MaxImageUrlLength;

        public int Id { get; set; }
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; }
        public string Text { get; set; }
        [MaxLength(FeaturedImageMaxLength)]
        public int? FeaturedImageId { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime? LastEdit { get; set; }
        [MaxLength(Values.IdentityIdMaxLength)]
        public string OwnerId { get; set; }

        public virtual User Owner { get; set; }
        public virtual Image? FeaturedImage { get; set; }
        public virtual ICollection<UserVote> Votes {  get; set; }

        public int GetId()
            => Id;

        public void SetId(int value)
            => Id = value;
    }

    public record PostRequestDto
    {
        public string Title { get; init; }
        public string Text { get; init; }
        public int? FeaturedImageUrl { get; init; }

    }
}
