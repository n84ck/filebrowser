using FileBrowser.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace FileBrowser.Repository
{
    public class FileBrowserRepository : IDataRepository
    {
        
        private readonly HttpClient client;

        public FileBrowserRepository(HttpClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<FileHandlingResult<string[]?>> ListFilesAsync()
        {
            try
            {
                using HttpResponseMessage response = await client.GetAsync("dokumentumok");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                    var res = await response.Content.ReadAsStringAsync();
                    var deserializedResult = JsonConvert.DeserializeObject<string[]>(res);
                    if (deserializedResult != null)
                    {
                        return new FileHandlingResult<string[]?>(true, deserializedResult);
                    }

                    return new FileHandlingResult<string[]?>(false, null, "Invalid format of response");
            }

            return new FileHandlingResult<string[]?>(false, null, response?.Content?.ToString() ?? "Error");

            }
            catch (Exception ex)
            {
                return new FileHandlingResult<string[]?>(false, null, ex.Message);
            }
        }

        public async Task<FileHandlingResult<Result<byte[]>?>> GetFileAsync(string filename)
        {
            try
            {
                using HttpResponseMessage response = await client.GetAsync($"dokumentumok/{filename}");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return await ComposeResultFile(response);
                }

                var errorResult = await response.Content.ReadAsStringAsync();
                return new FileHandlingResult<Result<byte[]>?>(false, null, errorResult ?? "Error");

            }
            catch (Exception ex)
            {
                return new FileHandlingResult<Result<byte[]>?>(false, null, ex.Message);
            }
        }

        //public async Task SaveFile(string filename)
        //{
        //    using HttpResponseMessage response = await client.GetAsync($"dokumentumok/fajl/{filename}");
        //    if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //    {
        //        var fileName = response.Content.Headers.ContentDisposition?.FileName;
        //        using var fs = new FileStream(@"C:\temp\a\" + fileName, FileMode.Create, FileAccess.Write, FileShare.None);
        //        await response.Content.CopyToAsync(fs);
        //    }
        //}

        public async Task<FileHandlingResult<Type?>> SendFileAsync(FileStream file, string filename)
        {
            using (var multipartFormContent = new MultipartFormDataContent())
            {
                var fileStreamContent = new StreamContent(file);   
                multipartFormContent.Add(fileStreamContent, name: "files", fileName: filename);

                var response = await client.PostAsync("dokumentumok", multipartFormContent);
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    return new FileHandlingResult<Type?>(true, null);
                }

                var badRequestResponse = await response.Content.ReadAsStringAsync();
                return new FileHandlingResult<Type?>(false, null, badRequestResponse);
            }
        }

        private static async Task<FileHandlingResult<Result<byte[]>?>> ComposeResultFile(HttpResponseMessage response)
        {
            var res = await response.Content.ReadAsStringAsync();
            var deserializedResult = JsonConvert.DeserializeObject<Result<string>>(res);
            if (deserializedResult != null)
            {
                byte[] bytes = Convert.FromBase64String(deserializedResult.Content);
                return new FileHandlingResult<Result<byte[]>?>(true, new Result<byte[]>(bytes, deserializedResult.Filename));
            }

            return new FileHandlingResult<Result<byte[]>?>(false, null, "Invalid response format");
        }
    }
}
