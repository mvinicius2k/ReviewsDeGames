using AutoMapper;
using ReviewsDeGames.Models;

namespace ReviewsDeGames
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserRegisterDto, User>();
            CreateMap<User, UserResponseDto>();


        }
    }
}
