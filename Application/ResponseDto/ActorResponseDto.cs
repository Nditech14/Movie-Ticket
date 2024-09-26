namespace Application.ResponseDto
{
    public class ActorResponseDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Biography { get; set; }
        public string? ImageFile { get; set; }
        public string MovieId { get; set; }
        //public List<FileEntityDto> Image { get; set; } = new List<FileEntityDto>();
        // public List<MovieDto> Movies { get; set; } = new List<MovieDto>();
    }
}
