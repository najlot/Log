using NajlotLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace LogTestApp
{
    class Program
    {
		static ILog log = LogBuilder
			.New
			.SetLogLevel(LogLevel.Debug, LogLevel.Warn)
			.AppendFileLog("app.log")
			.AppendConsoleLog()
			.Build();

		static long TestLogOnce()
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			for (int i = 0; i < 100000; i++)
			{
				log.Info(i);
			}

			sw.Stop();

			return sw.ElapsedMilliseconds;
		}

		private static void TestLog()
		{
			var msMittelwert = new List<int>();
			
			for (int i = 0; i < 1; i++)
			{
				var ms = TestLogOnce();
				msMittelwert.Add((int)ms);
				log.Warn(ms);
			}

			log.Warn("".PadLeft(15, '-'));

			long sum = 0;

			foreach (var val in msMittelwert)
			{
				sum += val;
			}
			
			log.Warn("".PadLeft(15, '-'));
			Console.WriteLine(sum / msMittelwert.Count);
			log.Warn("".PadLeft(15, '-'));

			Stopwatch sw = new Stopwatch();
			sw.Start();

			log.Flush();
			Console.WriteLine(sw.ElapsedMilliseconds);
		}
		
		static void Main(string[] args)
		{
			TestLog();
		}
	}
}
