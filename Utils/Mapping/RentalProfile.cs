using AutoMapper;
using Task2.Models.Documents;
using Task2.Models.Domain;

namespace Task2.Utils.Mapping;

public class RentalProfile : Profile
{
    public RentalProfile()
    {
        CreateMap<Rental, RentalDocument>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
            .ForMember(dest => dest.ScooterId, opt => opt.MapFrom(src => src.ScooterId.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ReverseMap()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (RentalStatus)src.Status));
    }
}