using Application.Dto;
using Application.ResponseDto;
using AutoMapper;
using Core.Entities;

namespace Application.Service
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {  // Movie mapping
            CreateMap<Movie, MovieDto>().ReverseMap();
            CreateMap<Movie, MovieCreationDto>().ReverseMap();
            CreateMap<Movie, MovieResponseDto>().ReverseMap();
            CreateMap<Cart, CartResponseDto>().ReverseMap();
            CreateMap<AddToCartDto, CartItem>()
                .ForMember(dest => dest.MovieId, opt => opt.MapFrom(src => src.MovieId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

            // Actor mapping
            CreateMap<Actor, ActorDto>().ReverseMap();
            CreateMap<Actor, ActorResponseDto>().ReverseMap();
            CreateMap<ActorDto, ActorResponseDto>().ReverseMap();
            CreateMap<Actor, ActorUpdateDto>().ReverseMap();  // Added this line to handle ActorUpdateDto

            // Producer mapping
            CreateMap<Producer, ProducerDto>().ReverseMap();
            CreateMap<Producer, ProducerResponseDto>().ReverseMap();
            CreateMap<Producer, UpdateProducerDto>().ReverseMap();

            // Cinema mapping
            CreateMap<Cinema, CinemaDto>().ReverseMap();
            CreateMap<Cinema, CinemaResponseDto>().ReverseMap();

            // FileEntity mapping
            CreateMap<FileEntity, FileEntityDto>().ReverseMap();
            CreateMap<UpdateMovieStatusDto, Movie>();

            CreateMap<CartItemDto, CartItemDto>().ReverseMap();
            //===================================================================================================================//

            CreateMap<CartItemDto, CartItemDto>().ReverseMap();
            CreateMap<ActorDtoz, Actor>().ReverseMap();
            CreateMap<MovieDtoz, MovieDtoz>().ReverseMap();
            CreateMap<ProducerDtoz, Producer>().ReverseMap();


            CreateMap<ActorDtoz, ActorDto>().ReverseMap();
            CreateMap<ProducerDtoz, ProducerDto>().ReverseMap();
            CreateMap<MovieDtoz, Movie>().ReverseMap();
            CreateMap<MovieImageUploadDto, Movie>().ReverseMap();
            CreateMap<ResponseMoviesDto, MovieResponseDto>().ReverseMap();



        }
    }
}
