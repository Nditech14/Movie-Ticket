namespace Application.Service.Abstraction
{
    public interface IFileService<T>
    {
        Task<bool> DeleteFileAsync(string fileId);
        Task<Stream> DownloadFileAsync(string fileId);
        Task<IEnumerable<T>> GetFilesAsync();
        Task<T> UploadFileAsync(Stream fileStream, string fileName);
    }
}