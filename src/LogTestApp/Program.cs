﻿using NajlotLog;
using NajlotLog.Middleware;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LogTestApp
{
    class Program
    {
		static Program()
		{
			LogConfigurator.Instance
				.AddConsoleAppender()
				.AddFileAppender("Test.log")
				.SetLogExecutionMiddleware(new SyncLogExecutionMiddleware())
				.SetLogLevel(LogLevel.Info);

			log = LoggerPool.Instance.GetLogger(typeof(Program));
		}

		static Logger log; // = LoggerPool.Instance.GetLogger(typeof(Program));

		static long TestLogOnce()
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			for (int i = 0; i < 1000; i++)
			{
				log.Info(i);
			}

			sw.Stop();

			return sw.ElapsedMilliseconds;
		}

		private static void TestLogTime()
		{
			var msMittelwert = new List<int>();
			
			for (int i = 0; i < 1; i++)
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
