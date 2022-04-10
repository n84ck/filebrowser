namespace FileBrowser.Models
{
    public class Result<T>
    {
        public T Content { get; }
        public string Filename { get; }
        public Result(T content, string filename)
        {
            Content = content;
            Filename = filename;
        }
    }
}
