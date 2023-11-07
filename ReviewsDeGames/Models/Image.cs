using ReviewsDeGames.Services;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace ReviewsDeGames.Models
{
    public class Image : IModel<int>
    {
        public const int FilenameMaxLength = 350;
        public int Id { get; set; }
        [MaxLength(FilenameMaxLength)]
        public string FileName { get; set; }
        public string OwnerId { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public virtual User Owner { get; set; }
        public virtual User? UsedByAvatar { get; set; }
        public virtual Post? UsedByPostAsFeatured { get; set; }

        

        public int GetId()
            => Id;

        public object[] GetIdKeys()
            => new object[] { Id };

        public void SetId(int value)
            => Id = value;
    }

    public record ImageRequestDto
    {
        internal string FileName => FormFile.FileName;
        public IFormFile FormFile { get; init; }
        
       
    }
    public record ImageResponseDto
    {
        public int Id { get; init; }
        public string FileName { get; init; }
        public string OwnerId { get; init; }
        public DateTime UploadedAt { get; init; }
        public string Url { get; set; }

    }
}
