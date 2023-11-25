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
        if (attributes.All(a => a.ConstructorArguments.IsDefaultOrEmpty))
        {
            var typeDeclarations = GeneratorUtilities.GetTypeDeclarations(type).ToList();
            typeDeclarations[0] = $"public {typeDeclarations[0]}Builder";

            return new(
                Namespace: GeneratorUtilities.GetNamespace(type),
                Name: type.Name + "Builder",
                FullType: type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                HintName: GeneratorUtilities.GetHintName(type) + "Builder",

                TypeDeclarations: new EquatableList<string>(typeDeclarations),
                Properties: new(GetAllProperties(type)),
                Constructors: new(type.InstanceConstructors.Where(c => c.DeclaredAccessibility == Accessibility.Public))
            );
        }

        var targetType = (INamedTypeSymbol)attributes.First().ConstructorArguments.First().Value!;

        return new(
            Namespace: GeneratorUtilities.GetNamespace(type),
            Name: type.Name,
            FullType: targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            HintName: GeneratorUtilities.GetHintName(type),

            TypeDeclarations: GeneratorUtilities.GetTypeDeclarations(type),
            Properties: new(GetAllProperties(targetType)),
            Constructors: new(targetType.InstanceConstructors.Where(c => c.DeclaredAccessibility == Accessibility.Public))
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
