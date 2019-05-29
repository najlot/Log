# Najlot.Log 

![Build status](https://dev.azure.com/Najlot/Log/_apis/build/status/Log%20msbuild?branchName=master) ![NuGet Version](https://img.shields.io/nuget/v/Najlot.Log.svg) ![alert_status](https://sonarcloud.io/api/project_badges/measure?project=najlot_Log&metric=alert_status) ![sqale_rating](https://sonarcloud.io/api/project_badges/measure?project=najlot_Log&metric=sqale_rating) ![reliability_rating](https://sonarcloud.io/api/project_badges/measure?project=najlot_Log&metric=reliability_rating) ![security_rating](https://sonarcloud.io/api/project_badges/measure?project=najlot_Log&metric=security_rating) ![sqale_index](https://sonarcloud.io/api/project_badges/measure?project=najlot_Log&metric=sqale_index) ![code_smells](https://sonarcloud.io/api/project_badges/measure?project=najlot_Log&metric=code_smells) ![duplicated_lines_density](https://sonarcloud.io/api/project_badges/measure?project=najlot_Log&metric=duplicated_lines_density) ![vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=najlot_Log&metric=vulnerabilities) ![ncloc](https://sonarcloud.io/api/project_badges/measure?project=najlot_Log&metric=ncloc) ![coverage](https://sonarcloud.io/api/project_badges/measure?project=najlot_Log&metric=coverage)

### Getting started:
You can start with just two lines of code:
```csharp
var log = LogAdminitrator.Instance.AddConsoleLogDestination().GetLogger(typeof(Program));
log.Info("Hello, World");
```

You can set the logging level on initialization and may change it later:
```csharp
LogAdminitrator.Instance
	.AddConsoleLogDestination(useColors: true)
	.SetLogLevel(LogLevel.Trace);

var log = LogAdminitrator.Instance.GetLogger(typeof(Program));

log.Info("Hello, World!");

LogAdminitrator.Instance.SetLogLevel(LogLevel.Warn);
log.Info("This message will not be logged.");
```

You tell the logger to log asynchronous (Putting the writing in a Task):
```csharp
LogAdminitrator.Instance
	.SetExecutionMiddleware<TaskExecutionMiddleware>()
	.AddConsoleLogDestination(useColors: true)
	.AddFileLogDestination("log.txt");
```

When you need something, then there is an example of a lot of things that are implemented:
```csharp
LogAdminitrator.Instance
	// Using synchronous or asynchronous middleware.
	// You can not use both. The last one gets applied.
	//.SetExecutionMiddleware<SyncExecutionMiddleware>()
	.SetExecutionMiddleware<TaskExecutionMiddleware>()

	// Tell the logger to write to a file and
	// calculate the file path it should write to.
	.AddFileLogDestination(() => $"{DateTime.UtcNow.ToString("yyyy-MM-dd")}.log")

	// Write to console using custom formatting and applying colors for different loglevels
	.AddConsoleLogDestination(useColors: true)
	.SetFormatMiddleware<ConsoleFormatMiddleware>(nameof(ConsoleLogDestination))

	// Add a destination implemented below.
	.AddCustomDestination(new DebugDestination())

	// Read configuration from an XML file,
	// write an example file when not found,
	// listen for and apply changes to all loggers, 
	// without restarting your application.
	.ReadConfigurationFromXmlFile("Najlot.Log.config",
		listenForChanges: true,
		writeExampleIfSourceDoesNotExists: true);

// Take specific logger.
Logger log = LogAdminitrator.Instance.GetLogger(typeof(Program));

// Begin a scope of work
using (log.BeginScope("start up"))
{
	log.Trace("Beginning to start...");

	try
	{
		throw new NotImplementedException();
	}
	catch (Exception ex)
	{
		// Log a fatal error with the exception that caused the error.
		log.Fatal(ex, "Could not start the application: ");
	}
}

log.Info("Press any key.");

Console.Read();

LogAdminitrator.Instance.Flush();

// Middleware used to format the output for console.
public class ConsoleFormatMiddleware : IFormatMiddleware
{
	public string Format(LogMessage message)
	{
		return $"{message.DateTime} {LogArgumentsParser.InsertArguments(message.Message, message.Arguments)} {message.Exception}";
	}
}

// Custom destination that writes to System.Diagnostics.Debug.
public class DebugDestination : ILogDestination
{
	public void Dispose()
	{
		// Nothing to do
	}

	public void Log(IEnumerable<LogMessage> messages, IFormatMiddleware formatMiddleware)
	{
		foreach (var message in messages)
		{
			System.Diagnostics.Debug.WriteLine(formatMiddleware.Format(message));
		}
	}
}
```

Najlot.Log has a provider for Microsoft.Extensions.Logging:
```csharp
using Microsoft.Extensions.Logging;
using Najlot.Log.Extensions.Logging;
using LogLevel = Najlot.Log.LogLevel;

...

var loggerFactory = new LoggerFactory();

loggerFactory.AddNajlotLog(administrator =>
{
	administrator
	.SetLogLevel(LogLevel.Info)
	.AddConsoleLogDestination()
	.AddFileLogDestination("log.txt");
});

var logger = loggerFactory.CreateLogger("default");
logger.LogInformation("Hello, World!");
```
