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
    public class ProducerServiceTests
    {
        private readonly Mock<ICosmosDbService<Producer>> _mockCosmosDbService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IFileRepository<FileEntity>> _mockFileService;
        private readonly ProducerService _producerService;

        public ProducerServiceTests()
        {
            _mockCosmosDbService = new Mock<ICosmosDbService<Producer>>();
            _mockMapper = new Mock<IMapper>();
            _mockFileService = new Mock<IFileRepository<FileEntity>>();
            _producerService = new ProducerService(_mockCosmosDbService.Object, _mockMapper.Object, _mockFileService.Object);
        }

        [Fact]
        public async Task AddProducerAsync_ShouldReturnProducer_WhenValidProducerDtoProvided()
        {
            // Arrange
            var producerDto = new ProducerDto
            {
                FullName = "Test Producer",
                Biography = "Test Biography",
                ImageFile = CreateMockImageFile() // Simulate image upload
            };

            var producer = new Producer
            {
                Id = Guid.NewGuid().ToString(),
                FullName = producerDto.FullName,
                Biography = producerDto.Biography
            };

            var fileEntity = new FileEntity { FileUrl = "https://example.com/image.jpg" };

            _mockMapper.Setup(x => x.Map<Producer>(It.IsAny<ProducerDto>()))
                .Returns(producer);

            _mockFileService.Setup(x => x.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(fileEntity);

            _mockCosmosDbService.Setup(x => x.AddItemAsync(It.IsAny<Producer>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mockMapper.Setup(x => x.Map<ProducerResponseDto>(It.IsAny<Producer>()))
                .Returns(new ProducerResponseDto { FullName = producerDto.FullName });

            // Act
            var result = await _producerService.AddProducerAsync(producerDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(producerDto.FullName, result.FullName);
            Assert.Equal("https://example.com/image.jpg", producer.ImageUrl);
        }



        [Fact]
        public async Task DeleteProducerAsync_ShouldReturnTrue_WhenProducerExists()
        {
            // Arrange
            var producerId = Guid.NewGuid().ToString();
            var existingProducer = new Producer { Id = producerId };

            _mockCosmosDbService.Setup(x => x.GetItemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(existingProducer);

            _mockCosmosDbService.Setup(x => x.DeleteItemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _producerService.DeleteProducerAsync(producerId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteProducerAsync_ShouldReturnFalse_WhenProducerDoesNotExist()
        {
            // Arrange
            var producerId = Guid.NewGuid().ToString();

            _mockCosmosDbService.Setup(x => x.GetItemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((Producer)null);

            // Act
            var result = await _producerService.DeleteProducerAsync(producerId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetAllProducersAsync_ShouldReturnListOfProducers()
        {
            // Arrange
            var producers = new List<Producer>
            {
                new Producer { Id = Guid.NewGuid().ToString(), FullName = "Producer 1" },
                new Producer { Id = Guid.NewGuid().ToString(), FullName = "Producer 2" }
            };

            var producerDtos = new List<ProducerResponseDto>
            {
                new ProducerResponseDto { FullName = "Producer 1" },
                new ProducerResponseDto { FullName = "Producer 2" }
            };

            _mockCosmosDbService.Setup(x => x.GetItemsAsync(It.IsAny<string>()))
                .ReturnsAsync(producers);

            _mockMapper.Setup(x => x.Map<IEnumerable<ProducerResponseDto>>(It.IsAny<IEnumerable<Producer>>()))
                .Returns(producerDtos);

            // Act
            var result = await _producerService.GetAllProducersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<ProducerResponseDto>)result).Count);
        }

        [Fact]
        public async Task GetProducerByIdAsync_ShouldReturnProducer_WhenProducerExists()
        {
            // Arrange
            var producerId = Guid.NewGuid().ToString();
            var producer = new Producer { Id = producerId, FullName = "Test Producer" };
            var producerDto = new ProducerResponseDto { FullName = "Test Producer" };

            _mockCosmosDbService.Setup(x => x.GetItemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(producer);

            _mockMapper.Setup(x => x.Map<ProducerResponseDto>(It.IsAny<Producer>()))
                .Returns(producerDto);

            // Act
            var result = await _producerService.GetProducerByIdAsync(producerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(producerDto.FullName, result.FullName);
        }

        [Fact]
        public async Task GetProducerByIdAsync_ShouldThrowException_WhenProducerDoesNotExist()
        {
            // Arrange
            var producerId = Guid.NewGuid().ToString();

            _mockCosmosDbService.Setup(x => x.GetItemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((Producer)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _producerService.GetProducerByIdAsync(producerId));
            Assert.Equal($"Producer with ID '{producerId}' not found.", exception.Message);
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
