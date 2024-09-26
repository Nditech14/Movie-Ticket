using Application.Service.Abstraction;
using Core.Entities;
using Infrastructure.Abstraction;

namespace Application.Service.Implementation
{
    public class FileService : IFileService<FileEntity>
    {
        private readonly IFileRepository<FileEntity> _fileRepository;

        public FileService(IFileRepository<FileEntity> fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task<FileEntity> UploadFileAsync(Stream fileStream, string fileName)
        {
            return await _fileRepository.UploadFileAsync(fileStream, fileName);
        }

        public async Task<Stream> DownloadFileAsync(string fileId)
        {
            return await _fileRepository.DownloadFileAsync(fileId);
        }

        public async Task<bool> DeleteFileAsync(string fileId)
        {
            return await _fileRepository.DeleteFileAsync(fileId);
        }

        public async Task<IEnumerable<FileEntity>> GetFilesAsync()
        {
            return await _fileRepository.GetFilesAsync();
        }

    }
}
