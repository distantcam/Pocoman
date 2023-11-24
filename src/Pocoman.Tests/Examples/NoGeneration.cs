using Pocoman;

[PocoBuilder]
public class NoGeneration
{
    public string GetterOnly { get; }
    public string ExpressionBodyProperty => nameof(ExpressionBodyProperty);

    public string SetterPrivate { get; private set; }
    public string SetterProtected { get; protected set; }
    private string PropertyPrivate { get; set; }
}
