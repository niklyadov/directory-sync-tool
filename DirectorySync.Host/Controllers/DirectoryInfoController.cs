using Microsoft.AspNetCore.Mvc;

namespace DirectorySync.Host.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DirectoryInfoController : ControllerBase
    {
        //private readonly DirectoryInfoService _directoryInfoService;
        private readonly ILogger<DirectoryInfoController> _logger;

        public DirectoryInfoController(ILogger<DirectoryInfoController> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            //_directoryInfoService = serviceProvider.GetRequiredService<DirectoryInfoService>();
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(DirectoryInfo.Scan($"{Props.SERVERPATH}"));
        }
    }
}