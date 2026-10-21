using AutoMapper;
using Task2.Models.Documents;
using Task2.Models.Domain;
using Task2.Models.DTO;

namespace Task2.Utils.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDocument>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.RentalHistory, opt => opt.MapFrom(src => src.RentalHistory));

        CreateMap<UserPostDto, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => MongoDB.Bson.ObjectId.GenerateNewId().ToString()))
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => 0.0m))
            .ForMember(dest => dest.BlockedDeposit, opt => opt.MapFrom(src => 0.0m))
            .ForMember(dest => dest.RentalHistory, opt => opt.MapFrom(src => new List<Rental>()));
    }
}