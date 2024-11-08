using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AvaloniaExtras.SourceGenerators.Abstractions;

public abstract class SourceGeneratorForFieldWithAttribute<TAttribute>
    : SourceGeneratorForMemberWithAttribute<TAttribute, VariableDeclaratorSyntax>
    where TAttribute : Attribute
{
    protected abstract string GenerateCode(
        Compilation compilation,
        FieldDeclarationSyntax node,
        IFieldSymbol symbol,
        TAttribute attribute,
        AnalyzerConfigOptions options
    );

    protected sealed override string GenerateCode(
        Compilation compilation,
        VariableDeclaratorSyntax node,
        ISymbol symbol,
        TAttribute attribute,
        AnalyzerConfigOptions options
    )
    {
        if (
            node.Parent is VariableDeclarationSyntax
            {
                Parent: FieldDeclarationSyntax fieldDeclarationSyntax
            }
        )
        {
            return GenerateCode(
                compilation,
                fieldDeclarationSyntax,
                (IFieldSymbol)symbol,
                attribute,
                options
            );
        }

        throw new InvalidCastException("Unexpected syntax node type");
    }

    protected sealed override bool IsSyntaxTarget(SyntaxNode node, CancellationToken _) =>
        node
            is VariableDeclaratorSyntax
            {
                Parent: VariableDeclarationSyntax
                {
                    Parent: FieldDeclarationSyntax { AttributeLists.Count: > 0 }
                }
            };
}
