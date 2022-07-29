using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCompilerClient.Services
{
    internal static class UserContextService
    {
        public static string Guid 
        { 
            get { return ConfigService.GetConfig("projectGuid"); }
            set { ConfigService.UpdateConfig("projectGuid", value); } 
        }

        public static string BaseUrl
        {
            get { return ConfigService.GetConfig("baseUrl"); }
            set { ConfigService.UpdateConfig("baseUrl", value); }
        }
    }
}
