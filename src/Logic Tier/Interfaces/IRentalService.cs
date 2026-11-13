using Task2.Models.Domain;
using Task2.Models.DTO.Business;
using Task2.Models.DTO.Patch;
using Task2.Models.DTO.Post;

namespace Task2.Logic_Tier.Interfaces;

public interface IRentalService
{
    Task<List<Rental>> GetAllAsync();
    Task<Rental?> GetByIdAsync(string id);
    Task<Rental> CreateAsync(RentalPostDto dto);
    Task UpdateAsync(string id, Rental rental);
    Task UpdatePartialAsync(string id, RentalPatchDto patchDto);
    Task DeleteAsync(string id);
    Task<Rental> StartRentalAsync(StartRentalDto dto);
    Task<Rental> FinishRentalAsync(FinishRentalDto dto);
    Task CancelRentalAsync(CancelRentalDto dto);
}
