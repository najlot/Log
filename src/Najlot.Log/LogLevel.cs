// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

namespace Najlot.Log
{
	/// <summary>
	/// Enum containing possible loglevels
	/// </summary>
	public enum LogLevel
	{
		/// <summary>
		/// Log every step that happens
		/// </summary>
		Trace = 0,

		/// <summary>
		/// Log messages that may help to find a bug
		/// </summary>
		Debug = 1,

		/// <summary>
		/// Log messages about "normal" things that happened
		/// </summary>
		Info = 2,

		/// <summary>
		/// Log things that should not be like that or should be changed, but still work
		/// </summary>
		Warn = 3,

		/// <summary>
		/// Log errors that can be recovered
		/// </summary>
		Error = 4,

		/// <summary>
		/// Log errors that "break" the application
		/// </summary>
		Fatal = 5,

		/// <summary>
		/// Log nothing
		/// </summary>
		None = 6
	}
}