using Pocoman;

public class External
{
    public required string IsRequired { get; init; }
    public string StringInit { get; init; }
    public int NumberInit { get; init; }
    public string Standard { get; set; }
}

[PocoBuilder(typeof(External))]
internal partial class ExternalBuilder
{
}
