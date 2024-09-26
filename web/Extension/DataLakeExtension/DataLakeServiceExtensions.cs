using Core.Entities;
using Infrastructure.Abstraction;
using Infrastructure.ADLS.configuration;

namespace YesMovie.Extension.DataLakeExtension
{
    public static class DataLakeServiceExtensions
    {
        public static void AddDataLakeServices(this IServiceCollection services, IConfiguration configuration)
        {
            var dataLakeSettings = configuration.GetSection("StorageSettings").Get<StorageSettings>();

            services.AddSingleton<IFileRepository<FileEntity>>(provider =>
                new AzureDataLakeFileRepository(dataLakeSettings.ConnectionString, dataLakeSettings.FileSystemName));


        }
    }
}
