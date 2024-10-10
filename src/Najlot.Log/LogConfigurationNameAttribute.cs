// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System;

namespace Najlot.Log;

/// <summary>
/// Implements class to name and property to name mapping for secure log-configuration.
/// Only classes with this attribute are allowed to be loaded
/// and only properties with this attribute are allowed to be changed.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = false)]
public sealed class LogConfigurationNameAttribute : Attribute
{
	public string Name { get; }

	public LogConfigurationNameAttribute(string name)
	{
		Name = name;
	}
}