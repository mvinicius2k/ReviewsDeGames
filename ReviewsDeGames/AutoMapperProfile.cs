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
            CreateMap<User, ImageResponseDto>();
            CreateMap<ImageRequestDto, Image>()
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(mp => mp.FileName));
            CreateMap<Image, ImageResponseDto>();


        }
    }
}
