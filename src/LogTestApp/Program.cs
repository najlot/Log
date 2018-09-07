using Najlot.Log;
using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LogTestApp
{
    class Program
    {
		static Logger log;

		static Program()
		{
			LogConfigurator.Instance
				.AddConsoleLogDestination()
				.AddFileLogDestination("Test.log")
				.SetExecutionMiddleware(new SyncExecutionMiddleware())
				.SetLogLevel(LogLevel.Fatal);

			log = LoggerPool.Instance.GetLogger(typeof(Program));
		}

		static long TestLogOnce()
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			for (int i = 0; i < 100000000; i++)
			{
				log.Info(i);
			}

			sw.Stop();

			return sw.ElapsedMilliseconds;
		}

		private static void TestLogTime()
		{
			var msMittelwert = new List<int>();
			
			for (int i = 0; i < 100; i++)
			{
				var ms = TestLogOnce();
				msMittelwert.Add((int)ms);
				log.Warn(ms);
				Console.WriteLine(ms);
			}
			
			long sum = 0;

			foreach (var val in msMittelwert)
			{
				sum += val;
			}

			Console.WriteLine("".PadLeft(16, '-'));
			Console.WriteLine(sum / msMittelwert.Count);
			Console.WriteLine("".PadLeft(16, '-'));

			Console.WriteLine("----FLUSHING----");
			Stopwatch sw = new Stopwatch();
			sw.Start();

			log.Flush();
			Console.WriteLine(sw.ElapsedMilliseconds);
			sw.Stop();
		}
		
		static void Main(string[] args)
		{
			TestLogTime();
		}
	}
}
