// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

using System;

namespace Najlot.Log
{
	public interface ILogger : IDisposable
	{
		IDisposable BeginScope<T>(T state);

		#region Normal logging

		void Trace<T>(T o);

		void Debug<T>(T o);

		void Info<T>(T o);

		void Warn<T>(T o);

		void Error<T>(T o);

		void Fatal<T>(T o);

		#endregion Normal logging

		#region Logging with exception

		void Trace<T>(Exception ex, T o);

		void Debug<T>(Exception ex, T o);

		void Info<T>(Exception ex, T o);

		void Warn<T>(Exception ex, T o);

		void Error<T>(Exception ex, T o);

		void Fatal<T>(Exception ex, T o);

		#endregion Logging with exception

		#region Structured logging

		void Trace(string s, params object[] args);

		void Debug(string s, params object[] args);

		void Info(string s, params object[] args);

		void Warn(string s, params object[] args);

		void Error(string s, params object[] args);

		void Fatal(string s, params object[] args);

		#endregion Structured logging

		#region Structured logging with exception

		void Trace(Exception ex, string s, params object[] args);

		void Debug(Exception ex, string s, params object[] args);

		void Info(Exception ex, string s, params object[] args);

		void Warn(Exception ex, string s, params object[] args);

		void Error(Exception ex, string s, params object[] args);

		void Fatal(Exception ex, string s, params object[] args);

		#endregion Structured logging with exception

		/// <summary>
		/// Tells the logger to write all messages to destinations, immediately.
		/// Will not return until done.
		/// </summary>
		void Flush();
	}
}