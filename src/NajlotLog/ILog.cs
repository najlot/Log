using System;

namespace NajlotLog
{
	public interface ILog
	{
		Action<object> Debug { get; }
		Action<object> Error { get; }
		Action<object> Fatal { get; }
		Action<object> Info { get; }
		Action<object> Warn { get; }

		void Flush();
	}
}