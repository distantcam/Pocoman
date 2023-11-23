using Microsoft.CodeAnalysis;


internal record struct TypeModel(
    string? Namespace,
    string Name,
    string HintName,

    EquatableList<IPropertySymbol> Properties
)
{
    public static TypeModel Create(INamedTypeSymbol type)
    {
        return new(
            Namespace: GeneratorUtilities.GetNamespace(type),
            Name: type.Name,
            HintName: GeneratorUtilities.GetHintName(type),

            Properties: new(type.GetMembers().OfType<IPropertySymbol>())
        );
    }
}
