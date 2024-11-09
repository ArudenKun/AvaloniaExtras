using System;
using System.Collections.Immutable;
using System.Threading;
using AvaloniaExtras.SourceGenerators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AvaloniaExtras.SourceGenerators.Abstractions;

public abstract class SourceGeneratorForFieldOrPropertyWithAttribute<TAttribute>
    : SourceGeneratorForMemberWithAttribute<TAttribute, SyntaxNode>
    where TAttribute : Attribute
{
    protected virtual string GenerateCode(
        Compilation compilation,
        VariableSyntax node,
        VariableSymbol symbol,
        AttributeData attribute,
        AnalyzerConfigOptions options
    ) => string.Empty;

    protected virtual string GenerateCode(
        Compilation compilation,
        VariableSyntax node,
        VariableSymbol symbol,
        ImmutableArray<AttributeData> attributes,
        AnalyzerConfigOptions options
    ) => string.Empty;

    protected sealed override string GenerateCode(
        Compilation compilation,
        SyntaxNode node,
        ISymbol symbol,
        AttributeData attribute,
        AnalyzerConfigOptions options
    ) => GenerateCode(compilation, ProcessNode(node), ProcessSymbol(symbol), attribute, options);

    protected sealed override string GenerateCode(
        Compilation compilation,
        SyntaxNode node,
        ISymbol symbol,
        ImmutableArray<AttributeData> attributes,
        AnalyzerConfigOptions options
    ) => GenerateCode(compilation, ProcessNode(node), ProcessSymbol(symbol), attributes, options);

    private static VariableSyntax ProcessNode(SyntaxNode node) =>
        node switch
        {
            PropertyDeclarationSyntax propertyDeclarationSyntax => propertyDeclarationSyntax,
            VariableDeclaratorSyntax
            {
                Parent: VariableDeclarationSyntax
                {
                    Parent: FieldDeclarationSyntax fieldDeclarationSyntax
                }
            } => fieldDeclarationSyntax,
            _ => throw new InvalidCastException(
                $"Unexpected syntax node type: {node.GetType().FullName}"
            ),
        };

    private static VariableSymbol ProcessSymbol(ISymbol symbol) =>
        symbol switch
        {
            IPropertySymbol propertySymbol => new VariableSymbol.Property(propertySymbol),
            IFieldSymbol fieldSymbol => new VariableSymbol.Field(fieldSymbol),
            _ => throw new InvalidCastException(
                $"Unexpected symbol type: {symbol.GetType().FullName}"
            ),
        };

    protected sealed override bool IsSyntaxTarget(SyntaxNode node, CancellationToken _) =>
        node
            is VariableDeclaratorSyntax
                {
                    Parent: VariableDeclarationSyntax
                    {
                        Parent: FieldDeclarationSyntax { AttributeLists.Count: > 0 }
                    }
                }
                or PropertyDeclarationSyntax { AttributeLists.Count: > 0 };
}
