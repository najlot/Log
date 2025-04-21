namespace Najlot.Log.Tests.Configuration;

public class HttpDestinationConfig
{
	public string Url { get; set; } = "http://localhost:5000/WriteLogs";
	public string Token { get; set; } = "";
}