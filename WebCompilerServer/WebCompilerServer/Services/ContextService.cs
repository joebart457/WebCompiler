using WebCompilerServer.Managers;
using WebCompilerServer.Models.Entities;

namespace WebCompilerServer.Services
{
    public static class ContextService
    {
        private static ConnectionManager? _connectionManager;
        public static ConnectionManager Connection { get { return Build(); } }

        public static ConnectionManager Build()
        {
            if (_connectionManager != null) return _connectionManager;
            _connectionManager = new ConnectionManager($"{ConfigService.AppConfig.DataDirectory}/data.db");
            _connectionManager.Register<ProjectInfoEntity>();
            return _connectionManager;
        }
    }
}
