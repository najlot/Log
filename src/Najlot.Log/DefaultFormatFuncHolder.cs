namespace Najlot.Log
{
	internal static class DefaultFormatFuncHolder
	{
		private const string _delimiter = " - ";

		internal static string DefaultFormatFunc(LogMessage message)
		{
			string timestamp = message.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
			var category = message.Category ?? "";
			string logLevel = message.LogLevel.ToString().ToUpper();

			if (logLevel.Length == 4)
			{
				logLevel += ' ';
			}

			var formatted = string.Concat(timestamp,
				_delimiter, logLevel,
				_delimiter, category,
				_delimiter, message.State,
				_delimiter, message.Message);

			return message.ExceptionIsValid ? formatted + message.Exception.ToString() : formatted;
		}
	}
}