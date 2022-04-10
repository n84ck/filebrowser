using FileBrowserAPI.Models;
using Microsoft.AspNetCore.StaticFiles;
using System.IO.Abstractions;

namespace FileBrowserAPI.Repository
{
    public class FileRepository : IDataRepository
    {
        private readonly string basePath;
        private readonly long maxFileSize;
        private readonly IFileSystem fileSystem;

        public FileRepository(IConfiguration configuration, IFileSystem fileSystem)
        {
            if (configuration == null) 
            { 
                throw new ArgumentNullException(nameof(configuration));
            }

            basePath = configuration.GetValue<string>("FilePath");
            maxFileSize = configuration.GetValue<long>("MaxFileSize");
            this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem)); ;
        }

        public async Task<FileHandlingResult<ResultFile?>> GetFileAsync(string filename)
        {
            filename = fileSystem.Path.GetFileName(filename);

            string filePath = fileSystem.Path.Combine(basePath, filename);

            if(!fileSystem.File.Exists(filePath))
            {
                return new FileHandlingResult<ResultFile?>(false, null, "The file does not exists");
            }

            string fileContentType = GetFileContentType(filePath);
            byte[] bytes = await fileSystem.File.ReadAllBytesAsync(filePath);
            var file = new ResultFile(bytes, filename, fileContentType);

            return new FileHandlingResult<ResultFile?>(true, file);
        }

        public async Task<FileHandlingResult<Base64Result?>> GetFileAsBase64Async(string filename)
        {
            var fileReadingResult = await this.GetFileAsync(filename);
            if(!fileReadingResult.IsSuccessful || fileReadingResult.Result is null)
            {
                return new FileHandlingResult<Base64Result?>(false, null, fileReadingResult.ErrorMessage);
            }

            var base64String = Convert.ToBase64String(fileReadingResult.Result.FileContent);
            return new FileHandlingResult<Base64Result?>(true, new Base64Result(base64String, fileReadingResult.Result.Filename));
        }

        public FileHandlingResult<string[]> GetAllFiles()
        {
            string[] files = fileSystem.Directory.GetFiles(basePath).Select(file => fileSystem.Path.GetFileName(file)).ToArray();
            return new FileHandlingResult<string[]>(true, files);
        }

        public async Task<FileHandlingResult<Type?>> StoreFileAsync(IFormFile file)
        {
            
            string filename = fileSystem.Path.GetFileName(file.FileName);
            string filePath = fileSystem.Path.Combine(basePath, filename);

            if(file.Length > maxFileSize) 
            {
                return new FileHandlingResult<Type?>(false, null, "The file is too large");
            }

            if (fileSystem.File.Exists(filePath)) 
            {
                return new FileHandlingResult<Type?>(false, null, "The file is already exists");
            }

            using Stream stream = fileSystem.File.Create(filePath);
            await file.CopyToAsync(stream);

            return new FileHandlingResult<Type?>(true, null);
        }

        private static string GetFileContentType(string filePath)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }
    }
}
