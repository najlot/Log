// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

namespace Najlot.Log.Middleware
{
	public interface IFilterMiddleware
	{
		bool AllowThrough(LogMessage message);
	}
}