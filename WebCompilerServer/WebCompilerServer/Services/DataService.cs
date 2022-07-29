namespace WebCompilerServer.Services
{
    public static class DataService
    {
        public static string ProjectTemplateText()
        {
            return Clean(@"
<Project Sdk=&qtMicrosoft.NET.Sdk&qt>
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <AssemblyName>$AssemblyName$</AssemblyName>
  </PropertyGroup>
</Project>");
        }

        private static string Clean(string src)
        {
            return src.Replace("&qt", "\"");
        }
    }
}
