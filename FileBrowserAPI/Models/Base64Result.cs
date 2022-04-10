namespace FileBrowserAPI.Models
{
    public class Base64Result
    {
        public string Content { get; }
        public string Filename { get; }
        public Base64Result(string content, string filename)
        {
            Content = content;
            Filename = filename;
        }
    }
}
