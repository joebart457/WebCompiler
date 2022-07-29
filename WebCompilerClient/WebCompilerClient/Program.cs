using cli.Builders;
using cli.Models;
using System;
using System.Linq;
using WebCompilerClient.Extensions;
using WebCompilerClient.Models;
using WebCompilerClient.Models.Enums;
using WebCompilerClient.Services;

namespace WebCompilerClient
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var main = CommandBuilder.CommandGroup("wcc")
				.ShowHelpIfUnresolvable()
				.Verb("create")
					.Option("p", "project")
						.Required()
						.WithValidator(s => !string.IsNullOrWhiteSpace(s))
					.Option("a", "assembly")
						.WithValidator(s => !string.IsNullOrWhiteSpace(s))
						.WithDefault("program")
					.LoggedAction(args =>
					{
						var name = args.ValueOf("p", "project");
						var asm = args.ValueOf("a", "assembly");
						var t = Client.Client.CreateProjectAsync(new ProjectManifest
						{
							AssemblyName = asm,
							ProjectName = name,
						});
						t.Wait();
						var results = t.Result;
						LoggerService.Log(t.Result.Guid, LogSeverity.SUCCESS);
						return 0;
					})
				.Verb("add")
					.Option("f", "filepath")
						.Required()
						.WithValidator(s => File.Exists(s))
					.LoggedAction(args =>
					{
						string filepath = args.ValueOf("f", "filepath");
						var t = Client.Client.AddFileAsync(UserContextService.Guid, filepath);
						t.Wait();
						LoggerService.Log($".SUCCESS {Path.GetFileName(filepath)} added to remote project {UserContextService.Guid}", LogSeverity.SUCCESS);
						return 0;
					})
				.Verb("build")
					.Option("r", "runtime")
						.Required()
					.LoggedAction(args =>
					{;
						string runtime = args.ValueOf("r", "runtime");

						if (!Enum.TryParse(typeof(Runtimes), runtime, out var runtimeSpecifier) || runtimeSpecifier == null)
							throw new ArgumentException($"cannot parse {runtime} to valid runtime specifier. Options are {string.Join(",", Enum.GetNames(typeof(Runtimes)))}");

						var t = Client.Client.BuildAsync(UserContextService.Guid, (Runtimes)runtimeSpecifier);
						t.Wait();
						LoggerService.Log($".Retreived build logs for project {UserContextService.Guid}:");
						LoggerService.Log(t.Result.Logs.ToList());
						LoggerService.Log(t.Result.ErrorLogs.ToList());

						return 0;
					})
				.Verb("download")
					.Option("f", "filepath")
						.Required()
						.WithValidator(s => !string.IsNullOrWhiteSpace(s))
					.LoggedAction(args =>
					{
						string filepath = args.ValueOf("f", "filepath");
						var t = Client.Client.DownloadResultsAsync(UserContextService.Guid, filepath);
						t.Wait();
						LoggerService.Log($".SUCCESS downloaded to path {filepath}", LogSeverity.SUCCESS);
						return 0;
					})
				.Verb("remote")
					.Option("s", "switch")
						.WithDefault("")
					.LoggedAction(args =>
					{
						var switchTo = args.ValueOf("s", "switch");
						if (string.IsNullOrEmpty(switchTo))
						{
							LoggerService.Log($".remote_url= {{{UserContextService.BaseUrl}}}");
						}
						else
						{
							UserContextService.BaseUrl = switchTo;
							LoggerService.Log($".remote_url set to {UserContextService.BaseUrl}");
						}

						return 0;
					})
				.Verb("project")
					.Option("s", "switch")
						.WithDefault("")
					.LoggedAction(args =>
					{
						var switchTo = args.ValueOf("s", "switch");
						if (string.IsNullOrEmpty(switchTo))
                        {
							LoggerService.Log($".current_project= {{{UserContextService.Guid}}}");
                        } else
                        {
							UserContextService.Guid = switchTo;
							LoggerService.Log($".current_project set to {UserContextService.Guid}");
						}
						
						return 0;
					});

			try
			{
				main.Execute(args);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}