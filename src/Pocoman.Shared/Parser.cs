using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pocoman;

public static class Parser
{
    public const string PocoBuilderAttributeFullName = "Pocoman.PocoBuilderAttribute";

    public static bool IsTypeDeclaration(SyntaxNode node, CancellationToken cancellationToken)
        => node is TypeDeclarationSyntax { AttributeLists.Count: > 0 };
}
