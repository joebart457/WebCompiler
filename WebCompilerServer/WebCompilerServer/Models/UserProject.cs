using WebCompilerServer.Models.Entities;

namespace WebCompilerServer.Models
{
    public class UserProject
    {
        public string Guid { get; set; } = "";
        public string Name { get; set; } = "";

        public UserProject() { }
        public UserProject(ProjectInfoEntity projectInfo)
        {
            Guid = projectInfo.Guid;
            Name = projectInfo.Alias;
        }
    }
}
