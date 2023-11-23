using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace Pocoman;

public sealed partial class PocomanGenerator
{
    private static class Parser
    {
        public const string PocoAttributeFullName = "Pocoman.PocoAttribute";

        public static bool IsTypeDeclaration(SyntaxNode node, CancellationToken cancellationToken)
            => node is TypeDeclarationSyntax { AttributeLists.Count: > 0 };
    }
}
