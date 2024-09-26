using Application.Dto;
using Application.ResponseDto;
using Core.Entities;

namespace Application.Service.Abstraction
{
    public interface IProducerService
    {
        Task<ProducerResponseDto> AddProducerAsync(ProducerDto producerDto);
        Task<ProducerResponseDto> AddProducerAsyncz(Producer producer);
        Task<bool> DeleteProducerAsync(string id);
        Task<IEnumerable<ProducerResponseDto>> GetAllProducersAsync();
        //  Task<ProducerResponseDto> GetProducerByFullNameAsync(string fullName);
        Task<ProducerResponseDto> GetProducerByIdAsync(string id);
        Task<ProducerResponseDto> UpdateProducerAsync(string id, UpdateProducerDto producerDto);
    }
}