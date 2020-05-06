// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

namespace Najlot.Log
{
	public interface ILogLevelObserver
	{
		void NotifyLogLevelChanged(LogLevel logLevel);
	}
}