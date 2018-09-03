using System;

namespace NajlotLog.Destinations
{
	/// <summary>
	/// Implementation of the prototype pattern with an extension in form of the "SourceType"
	/// </summary>
	/// <typeparam name="T">Log destination type</typeparam>
	public abstract class LogDestinationPrototype<T> where T : LogDestinationPrototype<T>, ILogger
	{
		/// <summary>
		/// Type the prototype was clonned for
		/// </summary>
		public Type SourceType { get; protected set; }

		/// <summary>
		/// Clones the log destination prototype
		/// </summary>
		/// <param name="sourceType">Type the prototype should be clonned for</param>
		/// <returns>Cloned log destination</returns>
		public T Clone(Type sourceType)
		{
			var cloned = this.MemberwiseClone() as T;
			cloned.SourceType = sourceType;
			return cloned;
		}
	}
}
