using FileBrowserAPI.Models;

namespace FileBrowserAPI.Repository

{
    public interface IDataRepository
    {
        /// <summary>
        /// List files in repository
        /// </summary>
        /// <returns>Filename array</returns>
        FileHandlingResult<string[]> GetAllFiles();

        /// <summary>
        /// Returns one file
        /// </summary>
        /// <param name="filename">The requested file name</param>
        /// <returns>Readed file</returns>
        Task<FileHandlingResult<ResultFile?>> GetFileAsync(string filename);

        /// <summary>
        /// Store file in repository
        /// </summary>
        /// <param name="file">The posted file</param>
        /// <returns>The result of storing</returns>
        Task<FileHandlingResult<Type?>> StoreFileAsync(IFormFile file);

        /// <summary>
        /// Returns the file as a base64 string
        /// </summary>
        /// <param name="filename">The name of the requested file</param>
        /// <returns>The result</returns>
        Task<FileHandlingResult<Base64Result?>> GetFileAsBase64Async(string filename);
    }
}
