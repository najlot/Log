﻿using System;

namespace NajlotLog.Util
{
	internal class OnDisposeExcecutor : IDisposable
	{
		private Action _action;

		public OnDisposeExcecutor(Action action)
		{
			_action = action;
		}

		public void Dispose()
		{
			_action();
		}
	}
}
