using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebCompilerServer.Models;
using WebCompilerServer.Models.Entities;
using WebCompilerServer.Services;
using WebCompilerServer.Extensions;
using WebCompilerServer.Models.Enums;

namespace WebCompilerServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(ILogger<ProjectsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("{guid}")]
        public ActionResult Get([FromRoute] string guid)
        {
            return this.WrapWithTryCatch(() =>
            {
                var projectInfo = ContextService.Connection.Select<ProjectInfoEntity>()
                    .Where(p => p.Guid == guid).FirstOrDefault();
                if (projectInfo == null) return NotFound();
                return Ok(new UserProject(projectInfo));
            });
        }

        [HttpPost]
        public ActionResult<UserProject> Create([FromBody] ProjectManifest manifest)
        {
            return this.WrapWithTryCatch(() =>
            {
                try
                {
                    var userProject = EnvironmentService.CreateNew(manifest);
                    return Ok(userProject);
                } catch(Exception ex)
                {
                    return Problem(ex.ToString());
                }
            });
        }

        [HttpGet]
        [Route("{guid}/build/{runtime}")]
        public ActionResult Build([FromRoute] string guid, [FromRoute] string runtime)
        {
            return this.WrapWithTryCatch(() =>
            {
                var projectInfo = ContextService.Connection.Select<ProjectInfoEntity>()
                                    .Where(p => p.Guid == guid).FirstOrDefault();
                if (projectInfo == null) return NotFound();
                if (!Enum.TryParse(typeof(Runtimes), runtime, out var runtimeSpecifier) || runtimeSpecifier == null)
                    throw new ArgumentException($"unable to parse {runtime} to a valid runtime specifier");
                return Ok(EnvironmentService.Build(projectInfo, (Runtimes)runtimeSpecifier));
            });
        }

        
    }
}
