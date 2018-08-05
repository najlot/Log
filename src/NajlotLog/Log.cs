using System;

namespace NajlotLog
{
	public class Log : ILog
	{
		private ILog Logger;

		internal static Action<object> NoAction = new Action<object>(o =>{});

		public Action<object> Debug { get; private set; } = NoAction;
		public Action<object> Info { get; private set; } = NoAction;
		public Action<object> Warn { get; private set; } = NoAction;
		public Action<object> Error { get; private set; } = NoAction;
		public Action<object> Fatal { get; private set; } = NoAction;

		internal Log(LogLevel logLevel, ILog log)
		{
			Logger = log ?? throw new ArgumentNullException(nameof(log));
			SetupLogLevel(logLevel);
		}
		
		private void SetupLogLevel(LogLevel logLevel)
		{
			switch (logLevel)
			{
				case LogLevel.Debug:
					Debug = new Action<object>(o => Logger.Debug(o));
					Info = new Action<object>(o => Logger.Info(o));
					Warn = new Action<object>(o => Logger.Warn(o));
					Error = new Action<object>(o => Logger.Error(o));
					Fatal = new Action<object>(o => Logger.Fatal(o));
					break;
				case LogLevel.Info:
					Info = new Action<object>(o => Logger.Info(o));
					Warn = new Action<object>(o => Logger.Warn(o));
					Error = new Action<object>(o => Logger.Error(o));
					Fatal = new Action<object>(o => Logger.Fatal(o));
					break;
				case LogLevel.Warn:
					Warn = new Action<object>(o => Logger.Warn(o));
					Error = new Action<object>(o => Logger.Error(o));
					Fatal = new Action<object>(o => Logger.Fatal(o));
					break;
				case LogLevel.Error:
					Error = new Action<object>(o => Logger.Error(o));
					Fatal = new Action<object>(o => Logger.Fatal(o));
					break;
				case LogLevel.Fatal:
					Fatal = new Action<object>(o => Logger.Fatal(o));
					break;
			}
		}
		
		public void Flush()
		{
			Logger.Flush();
		}
	}
}
