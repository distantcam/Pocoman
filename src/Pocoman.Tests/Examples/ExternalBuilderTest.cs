using Pocoman;

public class ExternalPerson
{
    public required string Name { get; init; }
    public string Address { get; init; }
    public int Age { get; init; }
    public string Nickname { get; set; }
}

[Poco(typeof(ExternalPerson))]
internal partial class ExternalPersonBuilder
{
}
