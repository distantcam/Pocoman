namespace Pocoman;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class PocoBuilderAttribute : Attribute
{
    public PocoBuilderAttribute()
    {
    }

    public PocoBuilderAttribute(Type builderType)
    {
    }
}
