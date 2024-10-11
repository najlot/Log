# Najlot.Log 
![NuGet Version](https://img.shields.io/nuget/v/Najlot.Log.svg)

</br>

## Getting started:
You can start with just two lines of code:
```csharp
var log = LogAdministrator.Instance.AddConsoleDestination().GetLogger(typeof(Program));
log.Info("Hello, World");
```

You can set the logging level on initialization and may change it later:
```csharp
LogAdministrator.Instance
	.AddConsoleDestination(useColors: true)
	.SetLogLevel(LogLevel.Trace);

var log = LogAdministrator.Instance.GetLogger(typeof(Program));

log.Info("Hello, World!");

LogAdministrator.Instance.SetLogLevel(LogLevel.Warn);
log.Info("This message will not be logged.");
```

You tell the logger to log asynchronous (Putting the writing on an other thread):
```csharp
LogAdministrator.Instance
    .SetCollectMiddleware<ConcurrentCollectMiddleware, FileDestination>()
    .SetCollectMiddleware<SyncCollectMiddleware, ConsoleDestination>()
    .AddConsoleDestination(useColors: true)
    .AddFileDestination("log.txt");
```

When you need something, then there is an example of a lot of things that are implemented:
```csharp
LogConfigurationMapper.Instance.AddToMapping<ConsoleFormatMiddleware>();
LogConfigurationMapper.Instance.AddToMapping<DebugDestination>();

LogAdministrator.Instance
	// Using synchronous or asynchronous middleware to collect messages.
	// You can not use both. The last one called gets applied.
	//.SetCollectMiddleware<SyncCollectMiddleware, FileDestination>()
	.SetCollectMiddleware<ConcurrentCollectMiddleware, FileDestination>()

	// Tell the logger to write to a file and
	// calculate the file path it should write to.
	.AddFileDestination("{Year}-{Month}-{Day}.log")

	// Write to console using custom formatting and applying colors for different loglevel
	.AddMiddleware<ConsoleFormatMiddleware, ConsoleDestination>()
	.AddConsoleDestination(useColors: true)
				
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
var log = LogAdministrator.Instance.GetLogger(typeof(Program));

// Begin a scope of work
using (log.BeginScope("start up"))
{
	log.Trace("Beginning to start...");

	try
	{
		throw new Exception();
	}
	catch (Exception ex)
	{
		// Log a fatal error with the exception that caused the error.
		log.Fatal(ex, "Exception test: ");
	}
}

log.Info("Press any key.");
log.Flush();
Console.Read();

// Middleware used to format the output for console.
[LogConfigurationName(nameof(ConsoleFormatMiddleware))]
public sealed class ConsoleFormatMiddleware : IMiddleware
{
	public IMiddleware NextMiddleware { get; set; }

	public void Execute(IEnumerable<LogMessage> messages)
	{
		foreach (var message in messages)
		{
			message.Message = $"{message.DateTime} {LogArgumentsParser.InsertArguments(message.RawMessage, message.Arguments)} {message.Exception}";
		}

		NextMiddleware.Execute(messages);
	}

	public void Flush()
	{
	}

	public void Dispose()
	{
	}
}

// Custom destination that writes to System.Diagnostics.Debug.
[LogConfigurationName(nameof(DebugDestination))]
public sealed class DebugDestination : IDestination
{
	public void Log(IEnumerable<LogMessage> messages)
	{
		foreach (var message in messages)
		{
			System.Diagnostics.Debug.WriteLine(message.Message);
		}
	}

	public void Flush()
	{
	}

	public void Dispose()
	{
	}
}
```

Najlot.Log has a provider for Microsoft.Extensions.Logging:
```csharp
using Microsoft.Extensions.Logging;
using Najlot.Log;
using Najlot.Log.Extensions.Logging;

...

var loggerFactory = new LoggerFactory();

loggerFactory.AddNajlotLog(administrator =>
{
	administrator
		.SetLogLevel(Najlot.Log.LogLevel.Info)
		.AddConsoleDestination(true)
		.AddFileDestination("log.txt");
});

var logger = loggerFactory.CreateLogger("default");
logger.LogInformation("Hello, World!");
```
