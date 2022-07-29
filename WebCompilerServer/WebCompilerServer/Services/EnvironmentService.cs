using WebCompilerServer.Models;
using WebCompilerServer.Models.Entities;
using System.Runtime.InteropServices;
using System.Diagnostics;
using WebCompilerServer.Models.Enums;
using System.IO.Compression;

namespace WebCompilerServer.Services
{
    public static class EnvironmentService
    {
        public static void SetupEnvironmentBase()
        {
            LoggerService.Log("Creating environment base...");
            Directory.CreateDirectory(ConfigService.AppConfig.DataDirectory);
            Directory.CreateDirectory(ConfigService.AppConfig.StageDirectory);
            Directory.CreateDirectory(ConfigService.AppConfig.BuildDirectory);
            Directory.CreateDirectory(ConfigService.AppConfig.ProjectDirectory);
            Directory.CreateDirectory(ConfigService.AppConfig.TemplateDirectory);
            File.WriteAllText(ConfigService.AppConfig.TemplateProjectPath, DataService.ProjectTemplateText());
        }
        public static UserProject CreateNew(ProjectManifest manifest)
        {
            if (manifest == null) throw new ArgumentNullException(nameof(manifest));
            SetupEnvironmentBase();
            var projectInfo = Scaffold(manifest);
            ContextService.Connection.Insert(projectInfo);
            return new UserProject(projectInfo);
        }

        public static async Task AddFiles(ProjectInfoEntity projectInfo, IFormFileCollection formFiles)
        {
            if (projectInfo == null) throw new ArgumentNullException(nameof(projectInfo));
            if (formFiles == null) throw new ArgumentNullException(nameof(formFiles));
            var tasks = new List<Task>();
            foreach(var file in formFiles)
            {
                LoggerService.Log($".Adding file {file.FileName} to project", guid: projectInfo.Guid);
                var fs = File.Create($"{projectInfo.RepositoryLocation}/{Path.GetFileName(file.FileName)}");
                tasks.Add(Task.Run(() => { file.CopyToAsync(fs); fs.Dispose(); }));
            }
            await Task.WhenAll(tasks);
        }


        public static BuildResult Build(ProjectInfoEntity projectInfo, Runtimes runtime)
        {
            if (projectInfo == null) throw new ArgumentNullException(nameof(projectInfo));
            try
            {
                LoggerService.Log($".Attempting to build project", guid: projectInfo.Guid);
                var argsPrepend = "-c";
                var shellName = "/bin/bash";
                var buildCmd = CreateBuildCommand(projectInfo, runtime);
                LoggerService.Log($"..Running build command: {buildCmd}", guid: projectInfo.Guid);
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    shellName = "cmd";
                    argsPrepend = "/c ";
                }
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = shellName,
                    Arguments = $"{argsPrepend} \"{buildCmd}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    WorkingDirectory = projectInfo.RepositoryLocation,
                };
                Process proc = new Process()
                {
                    StartInfo = startInfo,
                };
                proc.Start();
                proc.WaitForExit(88);
                var stdOut = proc.StandardOutput.ReadToEnd().Split('\n').Select(s => $"....{s}").ToList();
                var stdErr = proc.StandardError.ReadToEnd().Split('\n').Select(s => $".e..{s}").ToList();

                LoggerService.Log(stdOut, LogSeverity.LOGS);
                LoggerService.Log(stdErr, LogSeverity.WARNING);

                return new BuildResult
                {
                    Name = projectInfo.Alias,
                    Guid = projectInfo.Guid,
                    Logs = stdOut,
                    ErrorLogs = stdErr,
                    HadError = false,
                };
            }
            catch (Exception ex)
            {
                return new BuildResult
                {
                    Name = projectInfo.Alias,
                    Guid = projectInfo.Guid,
                    Logs = new List<string>(),
                    HadError = true,
                    ErrorTrace = ex.ToString(),
                };
            }
        }

        public static string PackageForDownload(ProjectInfoEntity projectInfo)
        {
            if (projectInfo == null) throw new ArgumentNullException(nameof(projectInfo));
            LoggerService.Log($".Packaging for download", guid: projectInfo.Guid);
            var destination = $"{ConfigService.AppConfig.StageDirectory}/{projectInfo.Guid}.zip";
            
            if (File.Exists(destination))
            {
                LoggerService.Log($"..destination file already exits, deleting", guid: projectInfo.Guid);
                File.Delete(destination);
            }

            ZipFile.CreateFromDirectory(projectInfo.OutputLocation, destination);
            return destination;
        }

        private static string CreateBuildCommand(ProjectInfoEntity projectInfo, Runtimes runtime)
        {
            switch (runtime)
            {
                case Runtimes.Linux_x64:
                    return $"dotnet publish -r linux-x64 --self-contained -o {projectInfo.OutputLocation}";
                case Runtimes.Windows_x64:
                    return $"dotnet publish -r win-x64 --self-contained -o {projectInfo.OutputLocation}";
                case Runtimes.OSX_x64:
                    return $"dotnet publish -r osx-x64 --self-contained -o {projectInfo.OutputLocation}";
                default:
                    throw new ArgumentException($"Runtime {runtime} is not supported");
            }
        }

        private static ProjectInfoEntity Scaffold(ProjectManifest manifest)
        {
            Guid projectGuid = Guid.NewGuid();
            LoggerService.Log($".Scaffolding new project: {projectGuid}", LogSeverity.SUCCESS);
            var projectDirectory = $"{ConfigService.AppConfig.ProjectDirectory}/{projectGuid}";
            var outputDirectory = $"{ConfigService.AppConfig.BuildDirectory}/{projectGuid}";
            
            Directory.CreateDirectory(projectDirectory);
            Directory.CreateDirectory(outputDirectory);

            var templatePath = ConfigService.AppConfig.TemplateProjectPath;
            var destPath = $"{projectDirectory}/project.csproj";
            
            File.WriteAllText(destPath, File.ReadAllText(templatePath).Replace("$AssemblyName$", manifest.AssemblyName));

            return new ProjectInfoEntity
            {
                Guid = projectGuid.ToString(),
                Alias = manifest.ProjectName,
                RepositoryLocation = projectDirectory,
                OutputLocation = outputDirectory,
            };
        }
    }
}
