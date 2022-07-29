using cli.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCompilerClient.Services;

namespace WebCompilerClient.Extensions
{
    public static class CommandGroupExtensions
    {
        public static CommandGroup LoggedAction(this CommandGroup group, Func<Args, int> func)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));
            if (func == null) throw new ArgumentNullException(nameof(func));
            group.Action(args =>
            {
                try
                {
                    return func(args);
                }
                catch (Exception ex)
                {
                    LoggerService.Log($"Unexpected error occurred", LogSeverity.ERROR, true);
                    LoggerService.Log($"ERROR -- {ex.Message}", LogSeverity.ERROR, true);

                }
                return 0;
            });
            return group;
        }
    }
}
