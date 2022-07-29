using WebCompilerServer.Managers;

namespace WebCompilerServer.Models.Entities
{
    public class ProjectInfoEntity
    {
        [Identity]
        public string Guid { get; set; } = "";
        public string Alias { get; set; } = "";
        public string RepositoryLocation { get; set; } = "";
        public string OutputLocation { get; set; } = "";
    }
}
