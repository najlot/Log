using System;

namespace Najlot.Log
{
	/// <summary>
	/// Implements class <-> name mapping, for secure log-configuration.
	/// Only classes with this attribute are allowed to be loaded.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class LogClassNameAttribute : Attribute
	{
		public string Name { get; }

		// TODO: name of this class may change
		public LogClassNameAttribute(string name)
		{
			Name = name;
		}
	}
}