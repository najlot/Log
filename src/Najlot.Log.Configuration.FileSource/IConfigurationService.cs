// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Configuration.FileSource.Models;

namespace Najlot.Log.Configuration.FileSource;

internal interface IConfigurationService
{
	Configurations ReadFromString(string content);

	string WriteToString(Configurations configurations);
}