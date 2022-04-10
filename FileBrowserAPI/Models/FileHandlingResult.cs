namespace FileBrowserAPI.Models
{
    public class FileHandlingResult<T>
    {
        public bool IsSuccessful { get; }
        public T Result { get; }
        public string ErrorMessage { get; }

        public FileHandlingResult(bool isSuccessful, T result, string errorMessage = "")
        {
            IsSuccessful = isSuccessful;
            Result = result;
            ErrorMessage = errorMessage;
        }
    }
}
