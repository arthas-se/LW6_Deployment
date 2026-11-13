using Moq;
using Task2.Data_Tier.Interfaces;
using Task2.Models.Domain;
using Task2.Services;

namespace TestProject.Tests;

public class RentalServiceTests
{
    private readonly Mock<IRentalRepository> _rentalRepoMock;
    private readonly Mock<IScooterRepository> _scooterRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<AutoMapper.IMapper> _mapperMock;
    private readonly RentalService _rentalService;

    public RentalServiceTests()
    {
        _rentalRepoMock = new Mock<IRentalRepository>();
        _scooterRepoMock = new Mock<IScooterRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<AutoMapper.IMapper>();

        _rentalService = new RentalService(
            _rentalRepoMock.Object,
            _scooterRepoMock.Object,
            _userRepoMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task GetAllAsync_ReturnsListOfRentals_WhenRentalsExist()
    {
        var rentals = new List<Rental> { new Rental { Id = "1" }, new Rental { Id = "2" } };
        _rentalRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(rentals);

        var result = await _rentalService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoRentalsExist()
    {
        _rentalRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Rental>());

        var result = await _rentalService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsRental_WhenIdExists()
    {
        var rental = new Rental { Id = "123", Status = RentalStatus.Active };
        _rentalRepoMock.Setup(r => r.GetByIdAsync("123")).ReturnsAsync(rental);

        var result = await _rentalService.GetByIdAsync("123");

        Assert.NotNull(result);
        Assert.Equal("123", result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenIdDoesNotExist()
    {
        _rentalRepoMock.Setup(r => r.GetByIdAsync("999")).ReturnsAsync((Rental?)null);

        var result = await _rentalService.GetByIdAsync("999");

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_MapsAndSavesRental()
    {
        var dto = new Task2.Models.DTO.Post.RentalPostDto();
        var rental = new Rental { Id = "1" };

        _mapperMock.Setup(m => m.Map<Rental>(dto)).Returns(rental);
        _rentalRepoMock.Setup(r => r.CreateAsync(rental)).Returns(Task.CompletedTask);

        var result = await _rentalService.CreateAsync(dto);

        Assert.NotNull(result);
        _rentalRepoMock.Verify(r => r.CreateAsync(rental), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ThrowsException_WhenMappingFails()
    {
        var dto = new Task2.Models.DTO.Post.RentalPostDto();
        _mapperMock.Setup(m => m.Map<Rental>(dto)).Throws<InvalidOperationException>();

        await Assert.ThrowsAsync<InvalidOperationException>(() => _rentalService.CreateAsync(dto));
    }

    [Fact]
    public async Task UpdateAsync_CallsRepositoryUpdate()
    {
        var rental = new Rental { Id = "1" };
        _rentalRepoMock.Setup(r => r.UpdateAsync("1", rental)).Returns(Task.CompletedTask);

        await _rentalService.UpdateAsync("1", rental);

        _rentalRepoMock.Verify(r => r.UpdateAsync("1", rental), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsException_WhenRepoFails()
    {
        var rental = new Rental { Id = "1" };
        _rentalRepoMock.Setup(r => r.UpdateAsync("1", rental)).ThrowsAsync(new Exception("DB Error"));

        await Assert.ThrowsAsync<Exception>(() => _rentalService.UpdateAsync("1", rental));
    }

    [Fact]
    public async Task UpdatePartialAsync_UpdatesStatus_WhenRentalExists()
    {
        var rental = new Rental { Id = "1", Status = RentalStatus.Active };
        var patchDto = new Task2.Models.DTO.Patch.RentalPatchDto { Status = (int)RentalStatus.Completed };

        _rentalRepoMock.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(rental);
        _rentalRepoMock.Setup(r => r.UpdateAsync("1", rental)).Returns(Task.CompletedTask);

        await _rentalService.UpdatePartialAsync("1", patchDto);

        Assert.Equal(RentalStatus.Completed, rental.Status);
        _rentalRepoMock.Verify(r => r.UpdateAsync("1", rental), Times.Once);
    }

    [Fact]
    public async Task UpdatePartialAsync_ThrowsKeyNotFoundException_WhenRentalMissing()
    {
        _rentalRepoMock.Setup(r => r.GetByIdAsync("999")).ReturnsAsync((Rental?)null);
        var patchDto = new Task2.Models.DTO.Patch.RentalPatchDto();

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _rentalService.UpdatePartialAsync("999", patchDto));
    }

    [Fact]
    public async Task DeleteAsync_CallsRepositoryDelete()
    {
        _rentalRepoMock.Setup(r => r.DeleteAsync("1")).Returns(Task.CompletedTask);

        await _rentalService.DeleteAsync("1");

        _rentalRepoMock.Verify(r => r.DeleteAsync("1"), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsException_WhenRepoFails()
    {
        _rentalRepoMock.Setup(r => r.DeleteAsync("1")).ThrowsAsync(new Exception("Error"));

        await Assert.ThrowsAsync<Exception>(() => _rentalService.DeleteAsync("1"));
    }

    [Fact]
    public async Task StartRentalAsync_CreatesRental_WhenConditionsAreMet()
    {
        var dto = new Task2.Models.DTO.Business.StartRentalDto { UserId = "u1", ScooterId = "s1" };
        var user = new User { Id = "u1", Balance = 100.0m };
        var scooter = new Scooter { Id = "s1", Status = ScooterStatus.Available, BatteryCharge = 80, Tariff = 2.0m };

        _userRepoMock.Setup(u => u.GetByIdAsync("u1")).ReturnsAsync(user);
        _scooterRepoMock.Setup(s => s.GetByIdAsync("s1")).ReturnsAsync(scooter);
        _rentalRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Rental>());

        var result = await _rentalService.StartRentalAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(RentalStatus.Active, result.Status);
        _rentalRepoMock.Verify(r => r.CreateAsync(It.IsAny<Rental>()), Times.Once);
    }

    [Fact]
    public async Task StartRentalAsync_ThrowsInvalidOperationException_WhenBatteryLow()
    {
        var dto = new Task2.Models.DTO.Business.StartRentalDto { UserId = "u1", ScooterId = "s1" };
        var user = new User { Id = "u1", Balance = 100.0m };
        var scooter = new Scooter { Id = "s1", Status = ScooterStatus.Available, BatteryCharge = 10, Tariff = 2.0m };

        _userRepoMock.Setup(u => u.GetByIdAsync("u1")).ReturnsAsync(user);
        _scooterRepoMock.Setup(s => s.GetByIdAsync("s1")).ReturnsAsync(scooter);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _rentalService.StartRentalAsync(dto));
    }

    [Fact]
    public async Task FinishRentalAsync_CompletesRental_WhenValid()
    {
        var dto = new Task2.Models.DTO.Business.FinishRentalDto { RentalId = "r1" };
        var rental = new Rental { Id = "r1", UserId = "u1", ScooterId = "s1", Status = RentalStatus.Active, StartTime = DateTime.UtcNow.AddMinutes(-10) };
        var user = new User { Id = "u1", Balance = 100.0m, RentalHistory = new List<Rental> { rental } };
        var scooter = new Scooter { Id = "s1", Status = ScooterStatus.InUse, BatteryCharge = 50, Tariff = 2.0m, RentalHistory = new List<Rental> { rental } };

        _rentalRepoMock.Setup(r => r.GetByIdAsync("r1")).ReturnsAsync(rental);
        _scooterRepoMock.Setup(s => s.GetByIdAsync("s1")).ReturnsAsync(scooter);
        _userRepoMock.Setup(u => u.GetByIdAsync("u1")).ReturnsAsync(user);

        var result = await _rentalService.FinishRentalAsync(dto);

        Assert.Equal(RentalStatus.Completed, result.Status);
        _rentalRepoMock.Verify(r => r.UpdateAsync("r1", rental), Times.Once);
    }

    [Fact]
    public async Task FinishRentalAsync_ThrowsKeyNotFoundException_WhenRentalNotFound()
    {
        var dto = new Task2.Models.DTO.Business.FinishRentalDto { RentalId = "invalid" };
        _rentalRepoMock.Setup(r => r.GetByIdAsync("invalid")).ReturnsAsync((Rental?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _rentalService.FinishRentalAsync(dto));
    }


    [Fact]
    public async Task CancelRentalAsync_CancelsRental_WhenWithinTimeLimit()
    {
        var dto = new Task2.Models.DTO.Business.CancelRentalDto { RentalId = "r1", CancellationReason = "Test" };
        var rental = new Rental { Id = "r1", UserId = "u1", ScooterId = "s1", Status = RentalStatus.Active, StartTime = DateTime.UtcNow.AddSeconds(30) };
        var user = new User { Id = "u1", Balance = 0m, RentalHistory = new List<Rental> { rental } };
        var scooter = new Scooter { Id = "s1", Status = ScooterStatus.InUse, RentalHistory = new List<Rental> { rental } };

        _rentalRepoMock.Setup(r => r.GetByIdAsync("r1")).ReturnsAsync(rental);
        _userRepoMock.Setup(u => u.GetByIdAsync("u1")).ReturnsAsync(user);
        _scooterRepoMock.Setup(s => s.GetByIdAsync("s1")).ReturnsAsync(scooter);

        await _rentalService.CancelRentalAsync(dto);

        Assert.Equal(RentalStatus.Canceled, rental.Status);
        _rentalRepoMock.Verify(r => r.UpdateAsync("r1", rental), Times.Once);
    }


    [Fact]
    public async Task CancelRentalAsync_ThrowsInvalidOperationException_WhenTimeLimitExceeded()
    {
        var dto = new Task2.Models.DTO.Business.CancelRentalDto { RentalId = "r1" };
        var rental = new Rental { Id = "r1", Status = RentalStatus.Active, StartTime = DateTime.UtcNow.AddMinutes(-5) };

        _rentalRepoMock.Setup(r => r.GetByIdAsync("r1")).ReturnsAsync(rental);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _rentalService.CancelRentalAsync(dto));
    }
}
