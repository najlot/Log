using System.IO;
using System.Text;

namespace Najlot.Log.Util
{
	public sealed class CustomStringWriter : StringWriter
	{
		private readonly Encoding _encoding;

		public CustomStringWriter(Encoding encoding)
		{
			_encoding = encoding;
		}

		public override Encoding Encoding { get { return _encoding; } }
	}
}