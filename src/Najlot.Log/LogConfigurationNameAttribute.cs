// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

using System;

namespace Najlot.Log
{
	/// <summary>
	/// Implements class <-> name mapping, for secure log-configuration.
	/// Only classes with this attribute are allowed to be loaded.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class LogConfigurationNameAttribute : Attribute
	{
		public string Name { get; }

		public LogConfigurationNameAttribute(string name)
		{
			Name = name;
		}
	}
}