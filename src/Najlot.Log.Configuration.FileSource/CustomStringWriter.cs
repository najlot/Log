﻿// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

using System.IO;
using System.Text;

namespace Najlot.Log.Configuration.FileSource
{
	internal sealed class CustomStringWriter : StringWriter
	{
		private readonly Encoding _encoding;

		public CustomStringWriter(Encoding encoding)
		{
			_encoding = encoding;
		}

		public override Encoding Encoding { get { return _encoding; } }
	}
}