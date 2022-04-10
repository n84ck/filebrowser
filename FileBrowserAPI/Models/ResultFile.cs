namespace FileBrowserAPI.Models
{
    public class ResultFile
    {
        public byte[] FileContent { get; }
        public string Filename { get; }
        public string ContentType { get; }

        public ResultFile(byte[] fileContent, string filename, string contentType)
        {
            FileContent = fileContent;
            Filename = filename;
            ContentType = contentType;
        }
    }
}
