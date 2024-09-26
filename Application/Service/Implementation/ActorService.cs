using Application.Dto;
using Application.ResponseDto;
using Application.Service.Abstraction;
using AutoMapper;
using Core.Entities;
using Infrastructure.Abstraction;

namespace Application.Service.Implementation
{
    public class ActorService : IActorService
    {
        private readonly ICosmosDbService<Actor> _cosmosDbService;
        private readonly IMapper _mapper;
        private readonly IFileRepository<FileEntity> _fileService;

        public ActorService(ICosmosDbService<Actor> cosmosDbService, IMapper mapper, IFileRepository<FileEntity> fileService)
        {
            _cosmosDbService = cosmosDbService;
            _mapper = mapper;
            _fileService = fileService;
        }

        // 1. Add a new actor with image upload
        public async Task<ActorResponseDto> AddActorAsync(ActorDto actorDto)
        {
            var actor = _mapper.Map<Actor>(actorDto);
            actor.Id = Guid.NewGuid().ToString(); // Assign a new ID

            // Handle image upload if provided
            if (actorDto.ImageFile != null)
            {
                var fileEntity = await _fileService.UploadFileAsync(actorDto.ImageFile.OpenReadStream(), actorDto.ImageFile.FileName);
                actor.ImageFile = fileEntity.FileUrl;  // Store the image URL
            }

            // Add the actor to Cosmos DB
            await _cosmosDbService.AddItemAsync(actor, actor.Id);  // Actor ID used as the partition key
            return _mapper.Map<ActorResponseDto>(actor);  // Return response DTO
        }

        // 2. Update an existing actor
        public async Task<ActorResponseDto> UpdateActorAsync(string id, ActorUpdateDto actorDto)
        {
            var existingActor = await _cosmosDbService.GetItemAsync(id, id);  // Actor ID is the partition key
            if (existingActor == null)
            {
                throw new Exception("Actor not found.");
            }

            _mapper.Map(actorDto, existingActor);  // Map new data to the existing actor

            // Handle image upload if provided
            if (actorDto.ImageFile != null)
            {
                var fileEntity = await _fileService.UploadFileAsync(actorDto.ImageFile.OpenReadStream(), actorDto.ImageFile.FileName);
                existingActor.ImageFile = fileEntity.FileUrl;  // Update the image URL
            }

            // Update the actor in Cosmos DB
            await _cosmosDbService.UpdateItemAsync(existingActor.Id, existingActor, existingActor.Id);  // Actor ID used as partition key
            return _mapper.Map<ActorResponseDto>(existingActor);  // Return response DTO
        }



        // 3. Delete an actor by ID
        public async Task<bool> DeleteActorAsync(string id)
        {
            var existingActor = await _cosmosDbService.GetItemAsync(id, id);  // Actor ID is the partition key
            if (existingActor == null)
            {
                return false;
            }

            await _cosmosDbService.DeleteItemAsync(id, id);  // Actor ID used as partition key
            return true;
        }

        // 4. Get all actors
        public async Task<IEnumerable<ActorResponseDto>> GetAllActorsAsync()
        {
            var actors = await _cosmosDbService.GetItemsAsync("SELECT * FROM c");
            return _mapper.Map<IEnumerable<ActorResponseDto>>(actors);  // Map and return the actor DTOs
        }

        // 5. Get an actor by ID
        public async Task<ActorResponseDto> GetActorByIdAsync(string id)
        {
            var actor = await _cosmosDbService.GetItemAsync(id, id);  // Actor ID is the partition key
            if (actor == null)
            {
                throw new Exception($"Actor with ID '{id}' not found.");
            }

            return _mapper.Map<ActorResponseDto>(actor);
        }

        // 6. Get actor by FullName
        public async Task<ActorResponseDto> GetActorByFullNameAsync(string fullName)
        {
            var query = $"SELECT * FROM c WHERE c.FullName = @fullName";
            var actors = await _cosmosDbService.GetItemsAsync(query);

            var actor = actors.FirstOrDefault();
            if (actor == null)
            {
                throw new Exception($"Actor with FullName '{fullName}' not found.");
            }

            return _mapper.Map<ActorResponseDto>(actor);
        }

        public async Task<ActorResponseDto> AddActorAsyncz(Actor actor)
        {
            // Generate a new ID for the Actor if it's not already set
            actor.Id = Guid.NewGuid().ToString();
            var actorz = _mapper.Map<ActorDto>(actor);
            // Handle image upload if the actor's image is provided
            if (actorz.ImageFile != null)
            {
                var fileEntity = await _fileService.UploadFileAsync(actorz.ImageFile.OpenReadStream(), actorz.ImageFile.FileName);
                actor.ImageFile = fileEntity.FileUrl;  // Store the image URL
            }
            var actorzz = _mapper.Map<Actor>(actorz);
            // Add the Actor entity to Cosmos DB
            await _cosmosDbService.AddItemAsync(actorzz, actorzz.Id);  // Actor ID is used as the partition key

            // Return a response DTO after successful addition
            return _mapper.Map<ActorResponseDto>(actorzz);
        }




    }
}
