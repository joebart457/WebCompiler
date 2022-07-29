using Microsoft.AspNetCore.Mvc;
using WebCompilerServer.Models;
using WebCompilerServer.Services;
using System.IO;
using WebCompilerServer.Extensions;
using WebCompilerServer.Models.Entities;

namespace WebCompilerServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CodeController : ControllerBase
    {
        private readonly ILogger<CodeController> _logger;

        public CodeController(ILogger<CodeController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("{guid}")]
        public async Task<ActionResult> Post([FromRoute] string guid)
        {
            return await this.WrapWithTryCatch(async () =>
            {
                var projectInfo = ContextService.Connection.Select<ProjectInfoEntity>().Where(x => x.Guid == guid).FirstOrDefault();
                if (projectInfo == null) return NotFound();
                await EnvironmentService.AddFiles(projectInfo, Request.Form.Files);
                return Ok();
            });
        }

       
    }
}
