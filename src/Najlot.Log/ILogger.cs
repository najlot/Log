// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System;

namespace Najlot.Log
{
	/// <summary>
	/// Common logger interface
	/// </summary>
	public interface ILogger
	{
		/// <summary>
		/// Starts a new scope that ends when you dispose the return value
		/// </summary>
		/// <typeparam name="T">Generic type</typeparam>
		/// <param name="state">Scope itself, will be converted to string</param>
		/// <returns></returns>
		IDisposable BeginScope<T>(T state);

		void Trace<T>(T o);

		void Trace<T>(Exception ex, T o);

		void Trace(string s, params object[] args);

		void Trace(Exception ex, string s, params object[] args);

		void Debug<T>(T o);

		void Debug<T>(Exception ex, T o);

		void Debug(string s, params object[] args);

		void Debug(Exception ex, string s, params object[] args);

		void Info<T>(T o);

		void Info<T>(Exception ex, T o);

		void Info(string s, params object[] args);

		void Info(Exception ex, string s, params object[] args);

		void Warn<T>(T o);

		void Warn<T>(Exception ex, T o);

		void Warn(string s, params object[] args);

		void Warn(Exception ex, string s, params object[] args);

		void Error<T>(T o);

		void Error<T>(Exception ex, T o);

		void Error(string s, params object[] args);

		void Error(Exception ex, string s, params object[] args);

		void Fatal<T>(T o);

		void Fatal<T>(Exception ex, T o);

		void Fatal(string s, params object[] args);

		void Fatal(Exception ex, string s, params object[] args);

		/// <summary>
		/// Tells the logger to write all messages to destinations, immediately.
		/// Will not return until done.
		/// </summary>
		void Flush();
	}
}