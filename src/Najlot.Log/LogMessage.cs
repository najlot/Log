// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Najlot.Log;

/// <summary>
/// The message to log
/// </summary>
public class LogMessage
{
	/// <summary>
	/// Timestamp the logging was requested
	/// </summary>
	public DateTime DateTime { get; set; }

	/// <summary>
	/// Logging level logging was requested with
	/// </summary>
	public LogLevel LogLevel { get; set; }

	/// <summary>
	/// Category the logger was requested for
	/// </summary>
	public string Category { get; set; }

	/// <summary>
	/// State set with BeginScope
	/// </summary>
	public object State { get; set; }

	/// <summary>
	/// The instance that was given to the request
	/// </summary>
	public string RawMessage { get; set; }

	/// <summary>
	/// Message formatted by a formating middleware
	/// </summary>
	public string Message { get; set; }

	/// <summary>
	/// Exception, if got any, otherwise null. Check ExceptionIsValid
	/// </summary>
	public Exception Exception { get; set; }

	/// <summary>
	/// Unparsed arguments received through one of the Trace / Debug / Info / Warn / Error / Fatal functions
	/// </summary>
	public IReadOnlyList<object> RawArguments { get; set; }

	/// <summary>
	/// Arguments received through one of the Trace / Debug / Info / Warn / Error / Fatal functions
	/// </summary>
	public IReadOnlyList<KeyValuePair<string, object>> Arguments { get; set; }
}