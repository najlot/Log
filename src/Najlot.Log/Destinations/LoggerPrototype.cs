using System;

namespace Najlot.Log.Destinations
{
	/// <summary>
	/// Implementation of the prototype pattern with an extension in form of the category
	/// </summary>
	/// <typeparam name="T">Log destination type</typeparam>
	public abstract class LogDestinationPrototype<T> where T : LogDestinationPrototype<T>, ILogger
	{
		/// <summary>
		/// Type the prototype was clonned for
		/// </summary>
		public string Category { get; protected set; }

		/// <summary>
		/// Clones the log destination prototype
		/// </summary>
		/// <param name="category">Type the prototype should be clonned for</param>
		/// <returns>Cloned log destination</returns>
		public T Clone(string category)
		{
			var cloned = this.MemberwiseClone() as T;
			cloned.Category = category;
			return cloned;
		}
	}
}
