using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ReviewsDeGames.Models
{
    public class User : IdentityUser
    {
        public const int UsernameMaxLenght = 150;
        public new string PasswordHash => "secret"; //Escondendo a senha de aparecer nas consultas odata

        public int? AvatarId { get; set; }

        public virtual Image Avatar { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<UserVote> Votes { get; set; }    
        public virtual ICollection<Post> Posts { get; set; }    
    }

    public record UserRegisterDto
    {
        public string UserName { get; init; }
        public string Email { get; init; }
        public int? AvatarId { get; init; }
    }

    public record UserResponseDto
    {
        public string Id { get; init; }
        public string UserName { get; init;}
        public string Email { get; init; }
        public int? AvatarId { get; init; }
    }
    public record PasswordVerificationDto
    {
        public bool Result { get; init; }
    }
    
}
