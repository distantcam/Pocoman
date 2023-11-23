using Microsoft.CodeAnalysis;

namespace Pocoman;

[Generator(LanguageNames.CSharp)]
public sealed partial class PocomanGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var types = context.SyntaxProvider.ForAttributeWithMetadataName(
            Parser.PocoAttributeFullName,
            Parser.IsTypeDeclaration,
            static (c, ct) => (INamedTypeSymbol)c.TargetSymbol)
        .Collect();

        context.RegisterSourceOutput(types, Emitter.GenerateSource);
    }
}
