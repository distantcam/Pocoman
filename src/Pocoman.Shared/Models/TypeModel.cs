using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

internal record struct TypeModel(
    string? Namespace,
    string Name,
    string FullType,
    string HintName,

    IReadOnlyList<string> TypeDeclarations,
    EquatableList<IPropertySymbol> Properties,
    EquatableList<IMethodSymbol> Constructors
) : IPartialTypeModel
{
    public static TypeModel Create(INamedTypeSymbol type, ImmutableArray<AttributeData> attributes)
    {
        var defaultAttribute = attributes.All(a => a.ConstructorArguments.IsDefaultOrEmpty);

        var targetType = defaultAttribute
            ? type
            : (INamedTypeSymbol)attributes.First().ConstructorArguments.First().Value!;
        var typeDeclarations = GeneratorUtilities.GetTypeDeclarations(type).ToList();
        var name =  type.Name;
        var hintName = GeneratorUtilities.GetHintName(type);

        if (defaultAttribute)
        {
            var lastIndex = typeDeclarations.Count - 1;
            typeDeclarations[lastIndex] = $"public {typeDeclarations[lastIndex]}Builder";
            name += "Builder";
            hintName += "Builder";
        }

        return new(
            Namespace: GeneratorUtilities.GetNamespace(type),
            Name: name,
            FullType: targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            HintName: hintName,

            TypeDeclarations: new EquatableList<string>(typeDeclarations),
            Properties: new(GetAllProperties(targetType)),
            Constructors: new(targetType.InstanceConstructors
                .Where(c => c.DeclaredAccessibility == Accessibility.Public))
        );
    }

    private static IEnumerable<IPropertySymbol> GetAllProperties(ITypeSymbol type)
    {
        var currentType = type;
        while (currentType != null)
        {
            foreach (var p in currentType.GetMembers().OfType<IPropertySymbol>().Where(p => p.DeclaredAccessibility == Accessibility.Public))
            {
                yield return p;
            }
            currentType = currentType.BaseType;
        }
    }
}
