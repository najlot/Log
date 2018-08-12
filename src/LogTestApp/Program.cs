using NajlotLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace LogTestApp
{
    class Program
    {
		static Log log = LogBuilder
			.New()
			.SetLogLevel(LogLevel.Fatal, LogLevel.Fatal)
			.AppendFileLog("app.log")
			.AppendConsoleLog()
			.Build();

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

		private static void TestLog()
		{
			var msMittelwert = new List<int>();
			
			for (int i = 0; i < 100; i++)
			{
				var ms = TestLogOnce();
				msMittelwert.Add((int)ms);
				log.Warn(ms);
				Console.WriteLine(ms);
			}

			log.Warn("".PadLeft(15, '-'));

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
			TestLog();
		}
	}
}
