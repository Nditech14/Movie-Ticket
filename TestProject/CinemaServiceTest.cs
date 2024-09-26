using Application.Dto;
using Application.Service.Abstraction;
using Application.Service.Implementation;
using AutoMapper;
using Core.Entities;
using Moq;
using System.Net;

namespace Application.Tests
{
    public class CinemaServiceTests
    {
        private readonly Mock<ICosmosDbService<Cinema>> _mockCinemaDbService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CinemaService _cinemaService;

        public CinemaServiceTests()
        {
            _mockCinemaDbService = new Mock<ICosmosDbService<Cinema>>();
            _mockMapper = new Mock<IMapper>();
            _cinemaService = new CinemaService(_mockCinemaDbService.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetCinemaByIdAsync_ShouldReturnSuccess_WhenCinemaExists()
        {
            // Arrange
            var cinemaId = Guid.NewGuid().ToString();
            var cinema = new Cinema { Id = cinemaId, CinemaName = "Test Cinema" };
            var cinemaDto = new CinemaDto { CinemaName = "Test Cinema" };

            _mockCinemaDbService.Setup(x => x.GetItemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(cinema);

            _mockMapper.Setup(x => x.Map<CinemaDto>(It.IsAny<Cinema>()))
                .Returns(cinemaDto);

            // Act
            var result = await _cinemaService.GetCinemaByIdAsync(cinemaId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
            Assert.Equal("Cinema retrieved successfully.", result.Message);
            Assert.Equal(cinemaDto.CinemaName, result.Data.CinemaName);
        }

        [Fact]
        public async Task GetCinemaByIdAsync_ShouldReturnNotFound_WhenCinemaDoesNotExist()
        {
            // Arrange
            var cinemaId = Guid.NewGuid().ToString();

            _mockCinemaDbService.Setup(x => x.GetItemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((Cinema)null);

            // Act
            var result = await _cinemaService.GetCinemaByIdAsync(cinemaId);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Equal((int)HttpStatusCode.NotFound, result.Code);
            Assert.Contains(result.Errors, e => e.Code == "NotFound");
        }

        [Fact]
        public async Task GetAllCinemasAsync_ShouldReturnCinemasList_WhenCinemasExist()
        {
            // Arrange
            var cinemas = new List<Cinema>
            {
                new Cinema { Id = Guid.NewGuid().ToString(), CinemaName = "Cinema 1" },
                new Cinema { Id = Guid.NewGuid().ToString(), CinemaName = "Cinema 2" }
            };

            var cinemaDtos = new List<CinemaDto>
            {
                new CinemaDto { CinemaName = "Cinema 1" },
                new CinemaDto { CinemaName = "Cinema 2" }
            };

            _mockCinemaDbService.Setup(x => x.GetItemsAsync(It.IsAny<string>()))
                .ReturnsAsync(cinemas);

            _mockMapper.Setup(x => x.Map<IEnumerable<CinemaDto>>(It.IsAny<IEnumerable<Cinema>>()))
                .Returns(cinemaDtos);

            // Act
            var result = await _cinemaService.GetAllCinemasAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<CinemaDto>)result).Count);
        }

        [Fact]
        public async Task UpdateCinemaNameAsync_ShouldReturnSuccess_WhenCinemaExists()
        {
            // Arrange
            var cinemaId = Guid.NewGuid().ToString();
            var cinema = new Cinema { Id = cinemaId, CinemaName = "Old Cinema" };
            var cinemaDto = new CinemaDto { CinemaName = "Updated Cinema" };

            _mockCinemaDbService.Setup(x => x.GetItemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(cinema);

            _mockCinemaDbService.Setup(x => x.UpdateItemAsync(It.IsAny<string>(), It.IsAny<Cinema>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mockMapper.Setup(x => x.Map<CinemaDto>(It.IsAny<Cinema>()))
                .Returns(cinemaDto);

            // Act
            var result = await _cinemaService.UpdateCinemaNameAsync(cinemaId, cinemaDto);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.Equal("Cinema updated successfully.", result.Message);
            Assert.Equal("Updated Cinema", result.Data.CinemaName);
        }

        [Fact]
        public async Task UpdateCinemaNameAsync_ShouldReturnNotFound_WhenCinemaDoesNotExist()
        {
            // Arrange
            var cinemaId = Guid.NewGuid().ToString();
            var cinemaDto = new CinemaDto { CinemaName = "Updated Cinema" };

            _mockCinemaDbService.Setup(x => x.GetItemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((Cinema)null);

            // Act
            var result = await _cinemaService.UpdateCinemaNameAsync(cinemaId, cinemaDto);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Equal((int)HttpStatusCode.NotFound, result.Code);
            Assert.Contains(result.Errors, e => e.Code == "NotFound");
        }

        [Fact]
        public async Task DeleteCinemaAsync_ShouldReturnTrue_WhenCinemaExists()
        {
            // Arrange
            var cinemaId = Guid.NewGuid().ToString();
            var cinema = new Cinema { Id = cinemaId };

            _mockCinemaDbService.Setup(x => x.GetItemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(cinema);

            _mockCinemaDbService.Setup(x => x.DeleteItemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _cinemaService.DeleteCinemaAsync(cinemaId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteCinemaAsync_ShouldReturnFalse_WhenCinemaDoesNotExist()
        {
            // Arrange
            var cinemaId = Guid.NewGuid().ToString();

            _mockCinemaDbService.Setup(x => x.GetItemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((Cinema)null);

            // Act
            var result = await _cinemaService.DeleteCinemaAsync(cinemaId);

            // Assert
            Assert.False(result);
        }


    }
}
