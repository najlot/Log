using System;
using System.Collections.Generic;
using System.Text;

namespace Najlot.Log.Util
{
	internal class DisposableListOfDisposables : List<IDisposable>, IDisposable
	{
		public void Dispose()
		{
			foreach (var item in this)
			{
				item.Dispose();
			}
		}
	}
}
