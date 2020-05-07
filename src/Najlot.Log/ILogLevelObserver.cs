// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

namespace Najlot.Log
{
	/// <summary>
	/// Interface for an observer that reacts when LogLevel changes
	/// </summary>
	public interface ILogLevelObserver
	{
		void NotifyLogLevelChanged(LogLevel logLevel);
	}
}