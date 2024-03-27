# What is WebNon-WebHostingExamples?

WebNon-WebHostingExamples contains four .NET 8 projects and four ASP.NET Core 8 projects, demonstrating how web apps and non-web apps can be hosted using [Host.CreateDefaultBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.host.createdefaultbuilder?view=dotnet-plat-ext-8.0), [Host.CreateApplicationBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.host.createapplicationbuilder?view=dotnet-plat-ext-8.0), and [WebApplication.CreateBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.webapplication.createbuilder?view=aspnetcore-8.0).

For comparison purposes, the API examples are functionally the same, but demonstrate different hosting methods.  Likewise with the console app examples.  The source code contains comments that highlight some of the differences and similarities between the hosting methods.


All example hosting projects demonstrate:
- Dependency injection.
- Loading/overriding of app settings from appsettings.json, appsettings.\{Environment\}.json, user secrets, environment variables, and command-line arguments.
- Strongly typed access to app settings using the [options pattern](https://learn.microsoft.com/en-us/dotnet/core/extensions/options), which also demonstrates how validation can be performed on app settings.
- Logging.

The following projects use the [built-in logging provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging-providers) to write to the console.  They also demonstrate validating app settings using Data Annotations.
- `Metalhead.Examples.Hosting.SimpleCDB.Api` Minimal API using `Host.CreateDefaultBuilder`.
- `Metalhead.Examples.Hosting.SimpleCDB.ConsoleApp` Console app using `Host.CreateDefaultBuilder`.
- `Metalhead.Examples.Hosting.SimpleCB.Api` Minimal API using `WebApplication.CreateBuilder`.
- `Metalhead.Examples.Hosting.SimpleCAB.ConsoleApp` Console app using `Host.CreateApplicationBuilder`.

The following projects use the [Serilog logging provider](https://serilog.net/) to write to the console and file.  They also demonstrate validating app settings using IValidateOptions\<TOptions\>, and how single-dash command-line arguments can be used to override app settings.
- `Metalhead.Examples.Hosting.CDB.Api` Minimal API using `Host.CreateDefaultBuilder`.
- `Metalhead.Examples.Hosting.CDB.ConsoleApp` Console app using `Host.CreateDefaultBuilder`.
- `Metalhead.Examples.Hosting.CB.Api` Minimal API using `WebApplication.CreateBuilder`.
- `Metalhead.Examples.Hosting.CAB.ConsoleApp` Console app using `Host.CreateApplicationBuilder`.			

Log files for the console apps are written to the build output directory.  Log files for the APIs are written to the project directory.

# How does it work?
Each of the hosts demonstrated ([Host.CreateDefaultBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.host.createdefaultbuilder?view=dotnet-plat-ext-8.0), [Host.CreateApplicationBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.host.createapplicationbuilder?view=dotnet-plat-ext-8.0), and [WebApplication.CreateBuilder](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.webapplication.createbuilder?view=aspnetcore-8.0)) load app settings as default.  Therefore, **it is often unnecessary to write additional code to load app settings**.

The host loads ***host*** configuration from the following sources (in order):
1. Environment variables
2. Command-line arguments

Next, the host loads ***app*** configuration from the following sources (in order):
1. `appsettings.json`
2. appsettings.\{Environment\}.json (e.g. `appsettings.Development.json`)
3. User secrets
4. Environment variables
5. Command-line arguments

Any values for settings that are set in subsequent sources override values set in previous sources. For example, if 'WindSpeedUnit' is set to 'MPH' in `appsettings.json`, but is set to 'KPH' via command-line arguments, then 'KPH' will be used.

# Setup instructions
1. Clone the WebNon-WebHostingExamples repository.
2. Open the .NET solution in Visual Studio 2022 (or a compatible alternative).
3. Update `appsettings.json` and `appsettings.Development.json` as necessary, e.g. path to log file.
4. Set one of the above projects as the startup project.
5. Build the solution and run!
