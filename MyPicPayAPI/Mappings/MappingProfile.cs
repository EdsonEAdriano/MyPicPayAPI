using AutoMapper;
using Newtonsoft.Json.Linq;
using SimplePicPay.Helpers;
using SimplePicPay.Models;
using SimplePicPay.ViewModels;
using System.Drawing;

namespace SimplePicPay.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserModel, UserViewModel>()
                .ForMember(dest => dest.Type, map => map.MapFrom(src => src.Type == UserType.Store ? "Lojista" : "Padrão"))
                .ReverseMap();

            CreateMap<TransactionModel, TransactionViewModel>()
                .ForMember(dest => dest.PayerName, map => map.MapFrom(src => src.Payer.Name))
                .ForMember(dest => dest.PayeeName, map => map.MapFrom(src => src.Payee.Name))
                .ForMember(dest => dest.Status, map => map.MapFrom(src => Enum.GetName(typeof(TransactionStatus), src.Status)))
                .ReverseMap();

        }
    }
}
