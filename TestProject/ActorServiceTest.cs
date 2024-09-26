using Application.Dto;
using Application.ResponseDto;
using Application.Service.Abstraction;
using Application.Service.Implementation;
using AutoMapper;
using Core.Entities;
using Infrastructure.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Moq;

namespace Application.Tests
{
    public class ActorServiceTests
    {
        private readonly Mock<ICosmosDbService<Actor>> _mockCosmosDbService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IFileRepository<FileEntity>> _mockFileService;
        private readonly ActorService _actorService;

        public ActorServiceTests()
        {
            _mockCosmosDbService = new Mock<ICosmosDbService<Actor>>();
            _mockMapper = new Mock<IMapper>();
            _mockFileService = new Mock<IFileRepository<FileEntity>>();
            _actorService = new ActorService(_mockCosmosDbService.Object, _mockMapper.Object, _mockFileService.Object);
        }

        [Fact]
        public async Task AddActorAsync_ShouldReturnActor_WhenValidActorDtoProvided()
        {
            // Arrange
            var actorDto = new ActorDto
            {
                FullName = "Test Actor",
                Biography = "Test Biography",
                ImageFile = CreateMockImageFile() // Simulate image upload
            };

            var actor = new Actor
            {
                Id = Guid.NewGuid().ToString(),
                FullName = actorDto.FullName,
                Biography = actorDto.Biography
            };

            var fileEntity = new FileEntity { FileUrl = "https://example.com/image.jpg" };

            _mockMapper.Setup(x => x.Map<Actor>(It.IsAny<ActorDto>()))
                .Returns(actor);

            _mockFileService.Setup(x => x.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(fileEntity);

            _mockCosmosDbService.Setup(x => x.AddItemAsync(It.IsAny<Actor>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mockMapper.Setup(x => x.Map<ActorResponseDto>(It.IsAny<Actor>()))
                .Returns(new ActorResponseDto { FullName = actorDto.FullName });

            // Act
            var result = await _actorService.AddActorAsync(actorDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(actorDto.FullName, result.FullName);
            Assert.Equal("https://example.com/image.jpg", actor.ImageFile);
        }




        [Fact]
        public async Task DeleteActorAsync_ShouldReturnTrue_WhenActorExists()
        {
            // Arrange
            var actorId = Guid.NewGuid().ToString();
            var existingActor = new Actor { Id = actorId };

            _mockCosmosDbService.Setup(x => x.GetItemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(existingActor);

            _mockCosmosDbService.Setup(x => x.DeleteItemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _actorService.DeleteActorAsync(actorId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteActorAsync_ShouldReturnFalse_WhenActorDoesNotExist()
        {
            // Arrange
            var actorId = Guid.NewGuid().ToString();

            _mockCosmosDbService.Setup(x => x.GetItemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((Actor)null);

            // Act
            var result = await _actorService.DeleteActorAsync(actorId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetAllActorsAsync_ShouldReturnListOfActors()
        {
            // Arrange
            var actors = new List<Actor>
            {
                new Actor { Id = Guid.NewGuid().ToString(), FullName = "Actor 1" },
                new Actor { Id = Guid.NewGuid().ToString(), FullName = "Actor 2" }
            };

            var actorDtos = new List<ActorResponseDto>
            {
                new ActorResponseDto { FullName = "Actor 1" },
                new ActorResponseDto { FullName = "Actor 2" }
            };

            _mockCosmosDbService.Setup(x => x.GetItemsAsync(It.IsAny<string>()))
                .ReturnsAsync(actors);

            _mockMapper.Setup(x => x.Map<IEnumerable<ActorResponseDto>>(It.IsAny<IEnumerable<Actor>>()))
                .Returns(actorDtos);

            // Act
            var result = await _actorService.GetAllActorsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<ActorResponseDto>)result).Count);
        }

        [Fact]
        public async Task GetActorByIdAsync_ShouldReturnActor_WhenActorExists()
        {
            // Arrange
            var actorId = Guid.NewGuid().ToString();
            var actor = new Actor { Id = actorId, FullName = "Test Actor" };
            var actorDto = new ActorResponseDto { FullName = "Test Actor" };

            _mockCosmosDbService.Setup(x => x.GetItemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(actor);

            _mockMapper.Setup(x => x.Map<ActorResponseDto>(It.IsAny<Actor>()))
                .Returns(actorDto);

            // Act
            var result = await _actorService.GetActorByIdAsync(actorId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(actorDto.FullName, result.FullName);
        }

        [Fact]
        public async Task GetActorByIdAsync_ShouldThrowException_WhenActorDoesNotExist()
        {
            // Arrange
            var actorId = Guid.NewGuid().ToString();

            _mockCosmosDbService.Setup(x => x.GetItemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((Actor)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _actorService.GetActorByIdAsync(actorId));
            Assert.Equal($"Actor with ID '{actorId}' not found.", exception.Message);
        }

        // Helper method to simulate image upload
        private IFormFile CreateMockImageFile()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write("dummy image content");
            writer.Flush();
            stream.Position = 0;

            return new FormFile(stream, 0, stream.Length, "ImageFile", "dummy.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }
    }
}
