using FileBrowserAPI.Models;
using FileBrowserAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FileBrowserAPI.Controllers
{
    [Route("api/dokumentumok")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDataRepository dataRepository;

        public DocumentController(IDataRepository dataRepository)
        {
            this.dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
        }

        // GET: api/dokumentumok
        [HttpGet]
        public IActionResult Get()
        {
            var result = dataRepository.GetAllFiles();
            if(result.IsSuccessful)
            {
                return Ok(result.Result);
            }

            return BadRequest(result.ErrorMessage);
        }

        // GET api/dokumentumok/5
        [HttpGet("{filename}")]
        public async Task<IActionResult> GetAsBase64Async(string filename)
        {
            var result = await dataRepository.GetFileAsBase64Async(filename);

            if (result.IsSuccessful && result.Result != null)
            {
                return Ok(result.Result);
            }

            return BadRequest(result.ErrorMessage);

        }

        // GET api/dokumentumok/fajl/5
        [HttpGet("fajl/{filename}")]
        public async Task<IActionResult> GetAsync(string filename)
        {
            var result = await dataRepository.GetFileAsync(filename);

            if(result.IsSuccessful && result.Result != null)
            {
                return File(result.Result.FileContent, result.Result.ContentType, result.Result.Filename);
            }

            return BadRequest(result.ErrorMessage);
            
        }

        //UploadFile([FromBody] string base64Filecontentstring)

        // POST api/dokumentumok
        [HttpPost]
        public async Task<IActionResult> PostAsync(List<IFormFile> files)
        {
            if(files == null || files.Count == 0)
            {
                return BadRequest("No file is attached. The parameter name must be 'files' ");
            }

            if(files.Count > 1)
            {
                return BadRequest("Currently only one file is accepted for uploading"); // AC points in email
            }

            var result = await dataRepository.StoreFileAsync(files[0]);

            if(result.IsSuccessful)
            {
                return Ok();
            }

            return BadRequest(result.ErrorMessage);
        }

    }
}
