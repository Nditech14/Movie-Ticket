namespace Core.Entities
{
    public class FileEntity
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }  // Optional: in case of URL-based uploading
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public Stream FileContent { get; set; }
    }
}