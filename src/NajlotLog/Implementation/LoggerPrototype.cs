using System;

namespace NajlotLog.Implementation
{
	public abstract class LoggerPrototype<T> where T : LoggerPrototype<T>, ILogger
	{
		public Type SourceType { get; protected set; }

		public T Clone(Type sourceType)
		{
			var cloned = this.MemberwiseClone() as T;
			cloned.SourceType = sourceType;
			return cloned;
		}
	}
}
