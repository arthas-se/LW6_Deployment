using AutoMapper;
using Task2.Models.Documents;
using Task2.Models.Domain;
using Task2.Models.DTO.Post;

namespace Task2.Utils.Mapping;

public class ScooterProfile : Profile
{
    public ScooterProfile()
    {
        CreateMap<Scooter, ScooterDocument>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.Coordinate, opt => opt.MapFrom(src => src.Coordinate))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.Coordinate, opt => opt.MapFrom(src => src.Coordinate))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (ScooterStatus)src.Status))
            .ReverseMap();

        CreateMap<LocationCoordinate, LocationCoordinateDocument>().ReverseMap();

        CreateMap<ScooterPostDto, Scooter>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => MongoDB.Bson.ObjectId.GenerateNewId().ToString()))
            .ForMember(dest => dest.BatteryCharge, opt => opt.MapFrom(src => 100))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ScooterStatus.Available))
            .ForMember(dest => dest.RentalHistory, opt => opt.MapFrom(src => new List<Rental>()));
    }
}