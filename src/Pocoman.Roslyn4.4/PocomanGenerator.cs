using Microsoft.CodeAnalysis;

namespace Pocoman;

[Generator(LanguageNames.CSharp)]
public sealed partial class PocomanGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var types = context.SyntaxProvider.ForAttributeWithMetadataName(
            Parser.PocoBuilderAttributeFullName,
            Parser.IsTypeDeclaration,
            static (c, ct) => TypeModel.Create((INamedTypeSymbol)c.TargetSymbol, c.Attributes))
        .Collect();

        context.RegisterSourceOutput(types, Emitter.GenerateSource);
    }
}
