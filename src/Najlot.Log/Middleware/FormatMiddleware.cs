namespace Najlot.Log.Middleware
{
	[LogConfigurationName(nameof(FormatMiddleware))]
	public class FormatMiddleware : IFormatMiddleware
	{
		private const string _delimiter = " - ";

		public string Format(LogMessage message)
		{
			string timestamp = message.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
			var category = message.Category ?? "";
			string logLevel = message.LogLevel.ToString().ToUpper();

			if (logLevel.Length == 4)
			{
				logLevel += ' ';
			}

			var messageString = message.Message;

			if (message.Arguments.Count > 0 && messageString.Length > 0)
			{
				messageString = LogArgumentsParser.InsertArguments(messageString, message.Arguments);
			}

			var formatted = string.Concat(timestamp,
				_delimiter, logLevel,
				_delimiter, category,
				_delimiter, message.State,
				_delimiter, messageString);

			return message.ExceptionIsValid ? formatted + message.Exception.ToString() : formatted;
		}
	}
}