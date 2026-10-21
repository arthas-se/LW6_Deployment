using AutoMapper;
using Task2.Models.Documents;
using Task2.Models.Domain;
using Task2.Models.DTO.Post;

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

        CreateMap<RentalPostDto, Rental>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => MongoDB.Bson.ObjectId.GenerateNewId().ToString()))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => (DateTime?)null))
            .ForMember(dest => dest.TotalCost, opt => opt.MapFrom(src => 0.0m))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => RentalStatus.Active));
    }
}