using Application.Dto;
using Application.ResponseDto;
using Application.Service.Abstraction;
using AutoMapper;
using Core.Entities;
using Infrastructure.Abstraction;

namespace Application.Service.Implementation
{
    public class ProducerService : IProducerService
    {
        private readonly ICosmosDbService<Producer> _cosmosDbService;
        private readonly IMapper _mapper;
        private readonly IFileRepository<FileEntity> _fileService;

        public ProducerService(ICosmosDbService<Producer> cosmosDbService, IMapper mapper, IFileRepository<FileEntity> fileService)
        {
            _cosmosDbService = cosmosDbService;
            _mapper = mapper;
            _fileService = fileService;
        }

        // 1. Add a new producer with image upload
        public async Task<ProducerResponseDto> AddProducerAsync(ProducerDto producerDto)
        {
            var producer = _mapper.Map<Producer>(producerDto);
            producer.Id = Guid.NewGuid().ToString();

            if (producerDto.ImageFile != null)
            {
                var fileEntity = await _fileService.UploadFileAsync(producerDto.ImageFile.OpenReadStream(), producerDto.ImageFile.FileName);
                producer.ImageUrl = fileEntity.FileUrl;  // Store the image URL
            }

            // Add the producer to Cosmos DB
            await _cosmosDbService.AddItemAsync(producer, producer.Id);
            return _mapper.Map<ProducerResponseDto>(producer);
        }

        // 2. Update an existing producer
        public async Task<ProducerResponseDto> UpdateProducerAsync(string id, UpdateProducerDto producerDto)
        {
            var existingProducer = await _cosmosDbService.GetItemAsync(id, id);  // Actor ID is the partition key
            if (existingProducer == null)
            {
                throw new Exception("Producer not found.");
            }

            _mapper.Map(producerDto, existingProducer);  // Map new data to the existing actor

            // Handle image upload if provided
            if (producerDto.ImageFile != null)
            {
                var fileEntity = await _fileService.UploadFileAsync(producerDto.ImageFile.OpenReadStream(), producerDto.ImageFile.FileName);
                existingProducer.ImageUrl = fileEntity.FileUrl;  // Update the image URL
            }

            // Update the actor in Cosmos DB
            await _cosmosDbService.UpdateItemAsync(existingProducer.Id, existingProducer, existingProducer.Id);  // Actor ID used as partition key
            return _mapper.Map<ProducerResponseDto>(existingProducer);  // Return response DTO
        }

        // 3. Delete a producer by ID
        public async Task<bool> DeleteProducerAsync(string id)
        {
            var existingProducer = await _cosmosDbService.GetItemAsync(id, id);
            if (existingProducer == null)
            {
                return false;
            }

            await _cosmosDbService.DeleteItemAsync(id, id);
            return true;
        }

        // 4. Get all producers
        public async Task<IEnumerable<ProducerResponseDto>> GetAllProducersAsync()
        {
            var producers = await _cosmosDbService.GetItemsAsync("SELECT * FROM c");
            return _mapper.Map<IEnumerable<ProducerResponseDto>>(producers);
        }

        // 5. Get a producer by ID
        public async Task<ProducerResponseDto> GetProducerByIdAsync(string id)
        {
            var producer = await _cosmosDbService.GetItemAsync(id, id);
            if (producer == null)
            {
                throw new Exception($"Producer with ID '{id}' not found.");
            }

            return _mapper.Map<ProducerResponseDto>(producer);
        }

        public async Task<ProducerResponseDto> AddProducerAsyncz(Producer producer)
        {
            // var producer = _mapper.Map<Producer>(producerDto);
            producer.Id = Guid.NewGuid().ToString();
            var producerz = _mapper.Map<ProducerDto>(producer);
            // Handle image upload if provided
            if (producerz.ImageFile != null)
            {
                var fileEntity = await _fileService.UploadFileAsync(producerz.ImageFile.OpenReadStream(), producerz.ImageFile.FileName);
                producer.ImageUrl = fileEntity.FileUrl;  // Store the image URL
            }
            var producerzz = _mapper.Map<Producer>(producerz);
            // Add the producer to Cosmos DB
            await _cosmosDbService.AddItemAsync(producerzz, producerzz.Id);
            return _mapper.Map<ProducerResponseDto>(producerzz);
        }


    }
}
