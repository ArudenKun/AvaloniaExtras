using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AvaloniaExtras.SourceGenerators.Abstractions;

public abstract class SourceGeneratorForTypeWithAttribute<TAttribute>
    : SourceGeneratorForMemberWithAttribute<TAttribute, TypeDeclarationSyntax>
    where TAttribute : Attribute
{
    protected virtual string GenerateCode(
        Compilation compilation,
        TypeDeclarationSyntax node,
        INamedTypeSymbol symbol,
        AttributeData attribute,
        AnalyzerConfigOptions options
    )
    {
        return string.Empty;
    }

    protected virtual string GenerateCode(
        Compilation compilation,
        TypeDeclarationSyntax node,
        INamedTypeSymbol symbol,
        ImmutableArray<AttributeData> attributes,
        AnalyzerConfigOptions options
    )
    {
        return string.Empty;
    }

    protected sealed override string GenerateCode(
        Compilation compilation,
        TypeDeclarationSyntax node,
        ISymbol symbol,
        AttributeData attribute,
        AnalyzerConfigOptions options
    ) => GenerateCode(compilation, node, (INamedTypeSymbol)symbol, attribute, options);

    protected sealed override string GenerateCode(
        Compilation compilation,
        TypeDeclarationSyntax node,
        ISymbol symbol,
        ImmutableArray<AttributeData> attributes,
        AnalyzerConfigOptions options
    ) => GenerateCode(compilation, node, (INamedTypeSymbol)symbol, attributes, options);
}
