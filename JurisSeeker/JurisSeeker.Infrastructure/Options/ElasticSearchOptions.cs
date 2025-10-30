namespace JurisSeeker.Infrastructure.Options;

public class ElasticSearchOptions
{
    public string Url { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string IndexName { get; set; } = null!;
}