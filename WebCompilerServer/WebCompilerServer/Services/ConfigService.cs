using Newtonsoft.Json;
using WebCompilerServer.Models;

namespace WebCompilerServer.Services
{
    public static class ConfigService
    {
        private static AppConfig? _appConfig = null;

        public static AppConfig AppConfig { get { return Build("config.json"); } }
        public static AppConfig Build(string configPath)
        {
            if (_appConfig != null) return _appConfig;
            return JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(configPath)) ?? new AppConfig();
        }
    }
}
