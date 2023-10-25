using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ReviewsDeGames.Models
{
    public class User : IdentityUser
    {
        public const int UsernameMaxLenght = 150;
        
        public const int AvatarMaxLenght = 150;

        [MaxLength(AvatarMaxLenght)]
        public string? AvatarUrl { get; set; }
    }

    public record UserRegisterDto
    {
        public string UserName { get; init; }
        public string Email { get; init; }
        public string AvatarUrl { get; init; }
    }

    public record UserResponseDto
    {
        public string Id { get; init; }
        public string UserName { get; init;}
        public string Email { get; init; }
        public string AvatarUrl { get; init; }
    }
    public record PasswordVerificationDto
    {
        public bool Result { get; init; }
    }
    
}
