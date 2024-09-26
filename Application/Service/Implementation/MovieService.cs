using Application.Dto;
using Application.ResponseDto;
using Application.Service.Abstraction;
using AutoMapper;
using Core.Entities;
using Infrastructure.Abstraction;
using System.Net;

namespace Application.Service.Implementation
{
    public class MovieService : IMovieService
    {
        private readonly ICosmosDbService<Movie> _cosmosDbService;
        private readonly ICosmosDbService<Actor> _actorsDbService;
        private readonly ICosmosDbService<Producer> _producerDbService;
        private readonly ICosmosDbService<Cinema> _cinemaDbService;
        private readonly IMapper _mapper;
        private readonly IFileRepository<FileEntity> _fileService;

        public MovieService(
            ICosmosDbService<Movie> cosmosDbService,
            IMapper mapper,
            IFileRepository<FileEntity> fileService,
            ICosmosDbService<Actor> actorsDbService,
            ICosmosDbService<Producer> producerDbService,
            ICosmosDbService<Cinema> cinemaDbService)
        {
            _cosmosDbService = cosmosDbService;
            _mapper = mapper;
            _fileService = fileService;
            _actorsDbService = actorsDbService;
            _producerDbService = producerDbService;
            _cinemaDbService = cinemaDbService;
        }

        public async Task<ResponseDto<MovieResponseDto>> AddMovieAsync(MovieCreationDto movieDto)
        {
            try
            {
                if (movieDto == null)
                    throw new ArgumentNullException(nameof(movieDto));

                var movie = new Movie
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = movieDto.Title,
                    Description = movieDto.Description,
                    Price = movieDto.Price,
                    ReleaseDate = movieDto.ReleaseDate,
                    Status = movieDto.Status,
                    Genre = movieDto.Genre,
                    Actors = new List<Actor>(),
                    Producers = new List<Producer>(),
                    Cinemas = new List<Cinema>()
                };

                var actorIds = new List<string>();
                var producerIds = new List<string>();
                var cinemaIds = new List<string>();

                foreach (var actorDto in movieDto.Actors)
                {
                    var actor = new Actor
                    {
                        Id = Guid.NewGuid().ToString(),
                        FullName = actorDto.FullName,
                        Biography = actorDto.Biography,
                        MovieId = movie.Id
                    };
                    actorIds.Add(actor.Id);
                    movie.Actors.Add(actor);
                    await _actorsDbService.AddItemAsyncz(actor);
                }

                foreach (var cinemaDto in movieDto.Cinemas)
                {
                    var cinema = new Cinema
                    {
                        Id = Guid.NewGuid().ToString(),
                        CinemaName = cinemaDto.CinemaName,
                        MovieId = movie.Id
                    };
                    cinemaIds.Add(cinema.Id);
                    movie.Cinemas.Add(cinema);
                    await _cinemaDbService.AddItemAsyncz(cinema);
                }

                foreach (var producerDto in movieDto.Producers)
                {
                    var producer = new Producer
                    {
                        Id = Guid.NewGuid().ToString(),
                        FullName = producerDto.FullName,
                        Biography = producerDto.Biography,
                        MovieId = movie.Id
                    };
                    producerIds.Add(producer.Id);
                    movie.Producers.Add(producer);
                    movie.ActorIds = actorIds;
                    movie.ProducerIds = producerIds;
                    await _producerDbService.AddItemAsyncz(producer);
                }

                await _cosmosDbService.AddItemAsyncz(movie);

                var movieResponseDto = new MovieResponseDto
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Description = movie.Description,
                    Price = movie.Price,
                    ReleaseDate = movie.ReleaseDate,
                    Status = movie.Status,
                    Genre = movie.Genre,
                    Actors = movie.Actors,
                    Cinemas = movie.Cinemas,
                    Producers = movie.Producers,
                    ImageUrl = movie.ImageUrl
                };

                return ResponseDto<MovieResponseDto>.Success(movieResponseDto, "Movie added successfully");
            }
            catch (Exception ex)
            {
                var errors = new List<Error> { new Error("AddMovieError", ex.Message) };
                return ResponseDto<MovieResponseDto>.Failure(errors, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<MovieResponseDto> UpdateMovieImageAsync(string id, MovieImageUploadDto updatemovieDto)
        {
            var existingMovie = await _cosmosDbService.GetItemAsync(id, id);
            if (existingMovie == null)
            {
                throw new Exception("Movie not found.");
            }

            _mapper.Map(updatemovieDto, existingMovie);

            if (updatemovieDto.ImageFile != null)
            {
                var fileEntity = await _fileService.UploadFileAsync(updatemovieDto.ImageFile.OpenReadStream(), updatemovieDto.ImageFile.FileName);
                existingMovie.ImageUrl = fileEntity.FileUrl;
            }

            await _cosmosDbService.UpdateItemAsync(existingMovie.Id, existingMovie, existingMovie.Id);
            return _mapper.Map<MovieResponseDto>(existingMovie);
        }

        public async Task<bool> DeleteMovieAsync(string id)
        {
            var existingMovie = await _cosmosDbService.GetItemAsync(id, id);
            if (existingMovie == null)
                return false;

            await _cosmosDbService.DeleteItemAsync(id, id);
            return true;
        }

        public async Task<IEnumerable<MovieResponseDto>> GetAllMoviesAsync()
        {
            var movies = await _cosmosDbService.GetItemsAsync("SELECT * FROM c");
            return _mapper.Map<IEnumerable<MovieResponseDto>>(movies);
        }

        public async Task<ResponseDto<MovieResponseDto>> SearchMovieWithName(MovieDto movieDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(movieDto.Title))
                {
                    var errors = new List<Error> { new Error("ValidationError", "Movie title cannot be empty.") };
                    return ResponseDto<MovieResponseDto>.Failure(errors, (int)HttpStatusCode.BadRequest);
                }

                var query = $"SELECT * FROM c WHERE CONTAINS(c.Title, '{movieDto.Title}', true)";
                var movies = await _cosmosDbService.GetItemsAsync(query);

                if (movies == null || !movies.Any())
                {
                    var errors = new List<Error> { new Error("NotFoundError", "No movies found with the specified title.") };
                    return ResponseDto<MovieResponseDto>.Failure(errors, (int)HttpStatusCode.NotFound);
                }

                var movie = movies.FirstOrDefault();
                var movieResponseDto = _mapper.Map<MovieResponseDto>(movie);

                return ResponseDto<MovieResponseDto>.Success(movieResponseDto, "Movie found successfully");
            }
            catch (Exception ex)
            {
                var errors = new List<Error> { new Error("SearchMovieError", ex.Message) };
                return ResponseDto<MovieResponseDto>.Failure(errors, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<(IEnumerable<MovieResponseDto> Items, string ContinuationToken)> GetMoviesWithPaginationAsync(string continuationToken, int maxItemCount = 30)
        {
            var (movies, newContinuationToken) = await _cosmosDbService.GetItemsWithContinuationTokenAsync(continuationToken, maxItemCount);
            var movieDtos = _mapper.Map<IEnumerable<MovieResponseDto>>(movies);
            return (movieDtos, newContinuationToken);
        }

        public async Task<MovieResponseDto> GetMovieByIdAsync(string id)
        {
            var movie = await _cosmosDbService.GetItemAsync(id, id);
            if (movie == null)
                throw new Exception($"Movie with ID '{id}' not found.");

            return _mapper.Map<MovieResponseDto>(movie);
        }

        public async Task<MovieResponseDto> UpdateMovieStatusAsync(string id, UpdateMovieStatusDto newStatusDto)
        {
            var existingMovie = await _cosmosDbService.GetItemAsync(id, id);
            if (existingMovie == null)
                throw new Exception("Movie not found.");

            existingMovie.Status = newStatusDto.Status;
            await _cosmosDbService.UpdateItemAsync(existingMovie.Id, existingMovie, existingMovie.Id);

            return _mapper.Map<MovieResponseDto>(existingMovie);
        }

        public async Task UpdateMoviesWithActorImageAsync(string actorId, string newImageUrl)
        {
            var query = $"SELECT * FROM c WHERE ARRAY_CONTAINS(c.Actors, {{'id': '{actorId}'}}, true)";
            var movies = await _cosmosDbService.GetItemsAsync(query);

            foreach (var movie in movies)
            {
                var actorInMovie = movie.Actors.FirstOrDefault(a => a.Id == actorId);
                if (actorInMovie != null)
                {
                    actorInMovie.ImageFile = newImageUrl;
                }

                await _cosmosDbService.UpdateItemAsync(movie.Id, movie, movie.Id);
            }
        }

        public async Task UpdateMoviesWithProducerImageAsync(string producerId, string newImageUrl)
        {
            var query = $"SELECT * FROM c WHERE ARRAY_CONTAINS(c.Producers, {{'id': '{producerId}'}}, true)";
            var movies = await _cosmosDbService.GetItemsAsync(query);

            foreach (var movie in movies)
            {
                var producerInMovie = movie.Producers.FirstOrDefault(p => p.Id == producerId);
                if (producerInMovie != null)
                {
                    producerInMovie.ImageUrl = newImageUrl;
                }

                await _cosmosDbService.UpdateItemAsync(movie.Id, movie, movie.Id);
            }
        }

        public async Task UpdateMoviesWithCinemaNameAsync(string cinemaId, string newCinemaName)
        {
            var query = $"SELECT * FROM c WHERE ARRAY_CONTAINS(c.Cinemas, {{'id': '{cinemaId}'}}, true)";
            var movies = await _cosmosDbService.GetItemsAsync(query);

            foreach (var movie in movies)
            {
                var cinemaInMovie = movie.Cinemas.FirstOrDefault(c => c.Id == cinemaId);
                if (cinemaInMovie != null)
                {
                    cinemaInMovie.CinemaName = newCinemaName;
                }

                await _cosmosDbService.UpdateItemAsync(movie.Id, movie, movie.Id);
            }
        }
    }
}
