using System.Threading;
using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace UnitTests;

public class TransferServiceTests
{
    [Theory, AutoMockData]
    public async Task GetAllTransfers_WhenTransfersExist_ShouldReturnMappedDtos(
        Mock<ITransferRepository> mockRepo,
        List<Transfer> transfers)
    {
        // Arrange
        mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(transfers);
        var sut = new TransferService(mockRepo.Object);

        // Act
        var result = await sut.GetAllTransfersAsync();

        // Assert
        result.Should().HaveCount(transfers.Count);
    }

    [Theory, AutoMockData]
    public async Task GetTransferById_WhenTransferExists_ShouldReturnDto(
        Mock<ITransferRepository> mockRepo)
    {
        // Arrange
        var transfer = new Transfer(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, 5_000_000m);
        mockRepo.Setup(r => r.GetByIdAsync(transfer.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transfer);
        var sut = new TransferService(mockRepo.Object);

        // Act
        var result = await sut.GetTransferByIdAsync(transfer.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(transfer.Id);
        result.PlayerId.Should().Be(transfer.PlayerId);
        result.Fee.Should().Be(transfer.Fee);
    }

    [Theory, AutoMockData]
    public async Task GetTransferById_WhenTransferDoesNotExist_ShouldReturnNull(
        Mock<ITransferRepository> mockRepo,
        Guid nonExistentId)
    {
        // Arrange
        mockRepo.Setup(r => r.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Transfer?)null);
        var sut = new TransferService(mockRepo.Object);

        // Act
        var result = await sut.GetTransferByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Theory, AutoMockData]
    public async Task GetTransfersByPlayer_WhenPlayerHasTransfers_ShouldReturnFilteredDtos(
        Mock<ITransferRepository> mockRepo)
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var transfers = new List<Transfer>
        {
            new Transfer(playerId, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, 1_000_000m),
            new Transfer(playerId, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-30), 2_000_000m)
        };
        mockRepo.Setup(r => r.GetByPlayerIdAsync(playerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transfers);
        var sut = new TransferService(mockRepo.Object);

        // Act
        var result = await sut.GetTransfersByPlayerAsync(playerId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.PlayerId == playerId);
    }

    [Theory, AutoMockData]
    public async Task GetTransfersByPlayer_WhenPlayerHasNoTransfers_ShouldReturnEmpty(
        Mock<ITransferRepository> mockRepo,
        Guid playerId)
    {
        // Arrange
        mockRepo.Setup(r => r.GetByPlayerIdAsync(playerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Transfer>());
        var sut = new TransferService(mockRepo.Object);

        // Act
        var result = await sut.GetTransfersByPlayerAsync(playerId);

        // Assert
        result.Should().BeEmpty();
    }

    [Theory, AutoMockData]
    public async Task CreateTransfer_WhenDtoIsValid_ShouldPersistAndReturnDto(
        Mock<ITransferRepository> mockRepo)
    {
        // Arrange
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Transfer>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        var sut = new TransferService(mockRepo.Object);
        var dto = new CreateTransferDto(
            PlayerId: Guid.NewGuid(),
            FromTeamId: Guid.NewGuid(),
            ToTeamId: Guid.NewGuid(),
            TransferDate: DateTime.UtcNow,
            Fee: 10_000_000m
        );

        // Act
        var result = await sut.CreateTransferAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.PlayerId.Should().Be(dto.PlayerId);
        result.FromTeamId.Should().Be(dto.FromTeamId);
        result.ToTeamId.Should().Be(dto.ToTeamId);
        result.Fee.Should().Be(dto.Fee);
        mockRepo.Verify(r => r.AddAsync(It.IsAny<Transfer>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory, AutoMockData]
    public async Task CreateTransfer_WhenFromTeamIdEqualsToTeamId_ShouldThrowArgumentException(
        Mock<ITransferRepository> mockRepo)
    {
        // Arrange
        var sut = new TransferService(mockRepo.Object);
        var sameTeamId = Guid.NewGuid();
        var dto = new CreateTransferDto(
            PlayerId: Guid.NewGuid(),
            FromTeamId: sameTeamId,
            ToTeamId: sameTeamId,
            TransferDate: DateTime.UtcNow,
            Fee: 0m
        );

        // Act & Assert
        await sut.Invoking(s => s.CreateTransferAsync(dto))
                 .Should().ThrowAsync<ArgumentException>()
                 .WithMessage("*FromTeamId and ToTeamId cannot be the same*");
    }

    [Theory, AutoMockData]
    public async Task CreateTransfer_WhenFeeIsNegative_ShouldThrowArgumentException(
        Mock<ITransferRepository> mockRepo)
    {
        // Arrange
        var sut = new TransferService(mockRepo.Object);
        var dto = new CreateTransferDto(
            PlayerId: Guid.NewGuid(),
            FromTeamId: Guid.NewGuid(),
            ToTeamId: Guid.NewGuid(),
            TransferDate: DateTime.UtcNow,
            Fee: -1m
        );

        // Act & Assert
        await sut.Invoking(s => s.CreateTransferAsync(dto))
                 .Should().ThrowAsync<ArgumentException>()
                 .WithMessage("*Fee cannot be negative*");
    }

    [Theory, AutoMockData]
    public async Task CreateTransfer_WhenPlayerIdIsEmpty_ShouldThrowArgumentException(
        Mock<ITransferRepository> mockRepo)
    {
        // Arrange
        var sut = new TransferService(mockRepo.Object);
        var dto = new CreateTransferDto(
            PlayerId: Guid.Empty,
            FromTeamId: Guid.NewGuid(),
            ToTeamId: Guid.NewGuid(),
            TransferDate: DateTime.UtcNow,
            Fee: 0m
        );

        // Act & Assert
        await sut.Invoking(s => s.CreateTransferAsync(dto))
                 .Should().ThrowAsync<ArgumentException>()
                 .WithMessage("*PlayerId cannot be empty*");
    }
}
