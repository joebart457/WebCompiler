namespace WebCompilerServer.Models
{
    public class AppConfig
    {
        public string ProjectDirectory { get; set; } = "";
        public string StageDirectory { get; set; } = "";
        public string BuildDirectory { get; set; } = "";
        public string DataDirectory { get; set; } = "";
        public string TemplateDirectory { get; set; } = "";
        public string TemplateProjectPath { get; set; } = "";
    }
}
