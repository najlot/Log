using System;
using System.Collections.Generic;
using System.Text;

namespace Najlot.Log.Util
{
	internal sealed class DisposableListOfDisposables : List<IDisposable>, IDisposable
	{
		bool _disposed = false;

		public void Dispose()
		{
			if(!_disposed)
			{
				_disposed = true;

				foreach (var item in this)
				{
					item.Dispose();
				}
			}
		}
	}
}
