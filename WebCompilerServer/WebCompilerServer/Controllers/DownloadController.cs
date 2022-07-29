using Microsoft.AspNetCore.Mvc;
using WebCompilerServer.Extensions;
using WebCompilerServer.Models.Entities;
using WebCompilerServer.Services;

namespace WebCompilerServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DownloadController : ControllerBase
    {
        private readonly ILogger<DownloadController> _logger;

        public DownloadController(ILogger<DownloadController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("{guid}")]
        public ActionResult Download([FromRoute] string guid)
        {
            return this.WrapWithTryCatch(() =>
            {
                var projectInfo = ContextService.Connection.Select<ProjectInfoEntity>()
                    .Where(p => p.Guid == guid).FirstOrDefault();
                if (projectInfo == null) return NotFound();
                var path = EnvironmentService.PackageForDownload(projectInfo);
                var stream = System.IO.File.OpenRead(path);
                return new FileStreamResult(stream, "application/octet-stream");
            });
        }
    }
}
