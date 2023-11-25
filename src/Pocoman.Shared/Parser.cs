using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pocoman;

public static class Parser
{
    public const string PocoBuilderAttributeFullName = "Pocoman.PocoBuilderAttribute";
    public const string PocoMappedAttributeFullName = "Pocoman.PocoMappedAttribute";

    public static bool IsTypeDeclaration(SyntaxNode node, CancellationToken cancellationToken)
        => node is TypeDeclarationSyntax { AttributeLists.Count: > 0 };

    public static bool IsFieldDeclaration(SyntaxNode node, CancellationToken cancellationToken)
        => node is VariableDeclaratorSyntax variableDeclarator
        && variableDeclarator.Parent is VariableDeclarationSyntax variableDeclaration
        && variableDeclaration.Parent is FieldDeclarationSyntax { AttributeLists.Count: > 0 };
}
