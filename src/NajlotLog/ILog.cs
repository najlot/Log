using System;

namespace NajlotLog
{
	public interface ILog
	{
		void Debug(object o);
		void Info(object o);
		void Warn(object o);
		void Error(object o);
		void Fatal(object o);

		void Flush();
	}
}
