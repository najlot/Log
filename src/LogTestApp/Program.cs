using Najlot.Log;
using Najlot.Log.Configuration;
using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Najlot.Log.Configuration.FileSource;

namespace LogTestApp
{
    class Program
    {
		static ILogConfiguration logConfiguration;
		static Logger log;

		static Program()
		{
			var configPath = "Najlot.Log.config";

			LogConfigurator.Instance
				.GetLogConfiguration(out logConfiguration)
				.AddCustomDestination(new ColorfulConsoleDestination(logConfiguration))
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.ReadConfigurationFromXmlFile(configPath, writeExampleIfSourceDoesNotExists: true);

			log = LoggerPool.Instance.GetLogger(typeof(Program));
		}

		static long TestLogOnce()
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			for (int i = 0; i < 100000000; i++)
			{
				log.Trace(i);
			}

			sw.Stop();

			return sw.ElapsedMilliseconds;
		}

		private static void TestLogTime()
		{
			var msMittelwert = new List<int>();
			
			for (int i = 0; i < 10; i++)
			{
				using (log.BeginScope(i))
				{
					var ms = TestLogOnce();
					msMittelwert.Add((int)ms);
					log.Debug(ms);
				}
			}
			
			long sum = 0;

			foreach (var val in msMittelwert)
			{
				sum += val;
			}

			using (log.BeginScope("Result"))
			{
				log.Info("".PadLeft(16, '-'));
				log.Info(sum / msMittelwert.Count);
				log.Info("".PadLeft(16, '-'));
			}
			
			log.Warn("----FLUSHING----");
			Stopwatch sw = new Stopwatch();
			sw.Start();

			log.Flush();
			log.Info(sw.ElapsedMilliseconds);
			sw.Stop();
		}
		
		static void Main(string[] args)
		{
			logConfiguration.LogLevel = LogLevel.Trace;
			log.Trace("Just a message to see trace color");
			logConfiguration.LogLevel = LogLevel.Debug;

			TestLogTime();
			
			try
			{
				throw new OperationCanceledException("Test exception");
			}
			catch (Exception ex)
			{
				log.Error("Done: ", ex);
			}

			log.Fatal("Press any key");

			//Console.ReadKey();

			while(true)
			{
				Thread.Sleep(250);

				log.Info("Info");

				Thread.Sleep(250);

				log.Warn("Warn");
			}

		}
	}
}
