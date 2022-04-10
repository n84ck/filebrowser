using FileBrowser.Models;

namespace FileBrowser.Repository
{
    public interface IDataRepository
    {
        /// <summary>
        /// Lists files
        /// </summary>
        /// <returns>File list array</returns>
        Task<FileHandlingResult<string[]?>> ListFilesAsync();

        /// <summary>
        /// Receive a file data and filename
        /// </summary>
        /// <param name="filename">requested filename</param>
        /// <returns>Request result</returns>
        Task<FileHandlingResult<Result<byte[]>?>> GetFileAsync(string filename);

        Task<FileHandlingResult<Type?>> SendFileAsync(FileStream file, string filename);
    }
}
