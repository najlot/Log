namespace Najlot.Log
{
	internal static class DefaultFormatFuncHolder
	{
		internal static string DefaultFormatFunc(LogMessage message)
		{
			string timestamp = message.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
			var category = message.Category ?? "";
			string delimiter = " - ";
			string logLevel = message.LogLevel.ToString().ToUpper();

			if (logLevel.Length == 4)
			{
				logLevel += ' ';
			}

			var formatted = string.Concat(timestamp,
				delimiter, logLevel,
				delimiter, category,
				delimiter, message.State,
				delimiter, message.Message);

			return message.ExceptionIsValid ? formatted + message.Exception.ToString() : formatted;
		}
	}
}