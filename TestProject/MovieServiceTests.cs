using Application.Dto;
using Application.ResponseDto;
using Application.Service.Abstraction;
using Application.Service.Implementation;
using AutoMapper;
using Core.Entities;
using Infrastructure.Abstraction;
using Moq;

namespace TestProject
{
    public class MovieServiceTests
    {
        private readonly Mock<ICosmosDbService<Movie>> _mockMovieDbService;
        private readonly Mock<ICosmosDbService<Actor>> _mockActorDbService;
        private readonly Mock<ICosmosDbService<Producer>> _mockProducerDbService;
        private readonly Mock<ICosmosDbService<Cinema>> _mockCinemaDbService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IFileRepository<FileEntity>> _mockFileService;
        private readonly MovieService _movieService;

        public MovieServiceTests()
        {
            _mockMovieDbService = new Mock<ICosmosDbService<Movie>>();
            _mockActorDbService = new Mock<ICosmosDbService<Actor>>();
            _mockProducerDbService = new Mock<ICosmosDbService<Producer>>();
            _mockCinemaDbService = new Mock<ICosmosDbService<Cinema>>();
            _mockMapper = new Mock<IMapper>();
            _mockFileService = new Mock<IFileRepository<FileEntity>>();

            _movieService = new MovieService(
                _mockMovieDbService.Object,
                _mockMapper.Object,
                _mockFileService.Object,
                _mockActorDbService.Object,
                _mockProducerDbService.Object,
                _mockCinemaDbService.Object
            );
        }

        [Fact]
        public async Task AddMovieAsync_ShouldReturnSuccess_WhenValidMovieCreationDtoProvided()
        {
            // Arrange
            var movieCreationDto = new MovieCreationDto
            {
                Title = "Test Movie",
                Description = "Test Description",
                Price = 10.0m,
                ReleaseDate = DateTime.UtcNow,
                Status = Core.Enum.MovieStatus.NowShowing,
                Genre = Core.Enum.Genre.Adventure,
                Actors = new List<ActorDtoz> { new ActorDtoz { FullName = "Actor 1", Biography = "Bio" } },
                Producers = new List<ProducerDtoz> { new ProducerDtoz { FullName = "Producer 1", Biography = "Bio" } },
                Cinemas = new List<CinemaDto> { new CinemaDto { CinemaName = "Cinema 1" } }
            };

            var movie = new Movie
            {
                Id = Guid.NewGuid().ToString(),
                Title = movieCreationDto.Title,
                Description = movieCreationDto.Description,
                Price = movieCreationDto.Price,
                ReleaseDate = movieCreationDto.ReleaseDate,
                Status = movieCreationDto.Status,
                Genre = movieCreationDto.Genre,
                Actors = new List<Actor>(),
                Producers = new List<Producer>(),
                Cinemas = new List<Cinema>()
            };

            // Act
            _mockMovieDbService.Setup(x => x.AddItemAsyncz(It.IsAny<Movie>())).Returns(Task.CompletedTask);
            _mockActorDbService.Setup(x => x.AddItemAsyncz(It.IsAny<Actor>())).Returns(Task.CompletedTask);
            _mockProducerDbService.Setup(x => x.AddItemAsyncz(It.IsAny<Producer>())).Returns(Task.CompletedTask);
            _mockCinemaDbService.Setup(x => x.AddItemAsyncz(It.IsAny<Cinema>())).Returns(Task.CompletedTask);

            var result = await _movieService.AddMovieAsync(movieCreationDto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
            Assert.Equal("Movie added successfully", result.Message);
        }



        [Fact]
        public async Task GetMovieByIdAsync_ShouldReturnMovie_WhenMovieExists()
        {
            // Arrange
            var movieId = Guid.NewGuid().ToString();
            var movie = new Movie { Id = movieId, Title = "Test Movie" };

            _mockMovieDbService.Setup(x => x.GetItemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(movie);

            _mockMapper.Setup(x => x.Map<MovieResponseDto>(It.IsAny<Movie>()))
                .Returns(new MovieResponseDto { Id = movieId, Title = movie.Title });

            // Act
            var result = await _movieService.GetMovieByIdAsync(movieId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(movieId, result.Id);
        }

        [Fact]
        public async Task GetMovieByIdAsync_ShouldThrowException_WhenMovieDoesNotExist()
        {
            // Arrange
            var movieId = Guid.NewGuid().ToString();

            _mockMovieDbService.Setup(x => x.GetItemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((Movie)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _movieService.GetMovieByIdAsync(movieId));
        }

        [Fact]
        public async Task DeleteMovieAsync_ShouldReturnTrue_WhenMovieExists()
        {
            // Arrange
            var movieId = Guid.NewGuid().ToString();
            var movie = new Movie { Id = movieId };

            _mockMovieDbService.Setup(x => x.GetItemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(movie);

            _mockMovieDbService.Setup(x => x.DeleteItemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _movieService.DeleteMovieAsync(movieId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteMovieAsync_ShouldReturnFalse_WhenMovieDoesNotExist()
        {
            // Arrange
            var movieId = Guid.NewGuid().ToString();

            _mockMovieDbService.Setup(x => x.GetItemAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((Movie)null);

            // Act
            var result = await _movieService.DeleteMovieAsync(movieId);

            // Assert
            Assert.False(result);
        }
    }
}