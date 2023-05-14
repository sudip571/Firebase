using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestRichard.Filters;
using TestRichard.Models;
using TestRichard.Services;

namespace TestRichard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadManagerController : ControllerBase
    {
        private readonly ILogger<DownloadManagerController> logger;
        private readonly IValidator<DownloadRequest> validator;
        private readonly IDownloadService downloadService;
        public DownloadManagerController(ILogger<DownloadManagerController> logger, IDownloadService downloadService, IValidator<DownloadRequest> validator)
        {
            this.logger = logger;
            this.downloadService = downloadService;
            this.validator = validator;
        }

        [HttpPost,Route("download")]       
        [ServiceFilter(typeof(ValidateModelStateFilter))]
        public async Task<IActionResult> DownloadFiles([FromBody] DownloadRequest request)
        {
            try
            {
                var response = await downloadService.Download(request);              
                return  Ok(response);
            }
            catch (Exception ex)
            {

               return BadRequest(ex.Message);
            }
        }
    }
}
