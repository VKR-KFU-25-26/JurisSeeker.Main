namespace JurisSeeker.Shared.Options;

public class ElasticSearchOptions
{
    public string Url { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string DefaultIndex { get; set; } = null!;
    public bool EnableDebugMode { get; set; } = false;
}