using Application.Dto;
using Application.ResponseDto;
using Core.Entities;

namespace Application.Service.Abstraction
{
    public interface IActorService
    {
        Task<ActorResponseDto> AddActorAsync(ActorDto actorDto);
        Task<ActorResponseDto> AddActorAsyncz(Actor actor);
        Task<bool> DeleteActorAsync(string id);
        Task<ActorResponseDto> GetActorByFullNameAsync(string fullName);
        Task<ActorResponseDto> GetActorByIdAsync(string id);
        Task<IEnumerable<ActorResponseDto>> GetAllActorsAsync();
        Task<ActorResponseDto> UpdateActorAsync(string id, ActorUpdateDto actorDto);
    }
}