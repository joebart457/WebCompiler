namespace WebCompilerClient.Models
{
    public class BuildResult
    {
        public string Guid { get; set; } = "";
        public string Name { get; set; } = "";
        public List<string> Logs { get; set; } = new List<string>();
        public List<string> ErrorLogs { get; set; } = new List<string>();
        public bool HadError { get; set; } = false;
        public string ErrorTrace { get; set; } = "";
    }
}
