namespace Infrastructure.Abstraction
{
    public interface IFileRepository<T>
    {
        Task<T> UploadFileAsync(Stream fileStream, string fileName);
        Task<Stream> DownloadFileAsync(string fileId);
        Task<bool> DeleteFileAsync(string fileId);
        Task<IEnumerable<T>> GetFilesAsync();
    }
}
