// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Globalization;

namespace Najlot.Log.Middleware
{
	[LogConfigurationName(nameof(FormatMiddleware))]
	public sealed class FormatMiddleware : IMiddleware
	{
		private const string _delimiter = " - ";
		private static readonly CultureInfo _enUsCultureInfo = new CultureInfo("en-US");

		public IMiddleware NextMiddleware { get; set; }

		public void Execute(IEnumerable<LogMessage> messages)
		{
			if (messages == null) return;

			foreach (var message in messages)
			{
				if (message.Message == null)
				{
					string timestamp = message.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff", _enUsCultureInfo);
					string logLevel = message.LogLevel.ToString().ToUpper(_enUsCultureInfo);

					if (logLevel.Length == 4)
					{
						logLevel += ' ';
					}

					var messageString = message.RawMessage;

					if (message.Arguments.Count > 0 && messageString.Length > 0)
					{
						messageString = LogArgumentsParser.InsertArguments(messageString, message.Arguments);
					}

					var formatted = string.Concat(timestamp,
						_delimiter, logLevel,
						_delimiter, message.Category,
						_delimiter, message.State?.ToString(),
						_delimiter, messageString);

					message.Message = message.ExceptionIsValid ? formatted + message.Exception.ToString() : formatted;
				}
			}

			NextMiddleware.Execute(messages);
		}

		public void Flush()
		{
		}

		public void Dispose() => Flush();
	}
}