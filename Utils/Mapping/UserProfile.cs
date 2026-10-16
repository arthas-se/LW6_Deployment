using AutoMapper;
using Task2.Models.Documents;
using Task2.Models.Domain;

namespace Task2.Utils.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDocument>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.RentalHistory, opt => opt.MapFrom(src => src.RentalHistory));
    }
}