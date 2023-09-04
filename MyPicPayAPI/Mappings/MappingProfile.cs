using AutoMapper;
using Newtonsoft.Json.Linq;
using SimplePicPay.Helpers;
using SimplePicPay.Models;
using SimplePicPay.ViewModels;

namespace SimplePicPay.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserModel, UserViewModel>()
                .ForMember(dest => dest.Type, map => map.MapFrom(src => src.Type == UserType.Store ? "Lojista" : "Padrão"))
                .ReverseMap();
        }
    }
}
