using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AvaloniaExtras.SourceGenerators.Abstractions;

public abstract class SourceGeneratorForDelegateWithAttribute<TAttribute>
    : SourceGeneratorForMemberWithAttribute<TAttribute, DelegateDeclarationSyntax>
    where TAttribute : Attribute
{
    protected virtual string GenerateCode(
        Compilation compilation,
        DelegateDeclarationSyntax node,
        INamedTypeSymbol symbol,
        AttributeData attribute,
        AnalyzerConfigOptions options
    ) => string.Empty;

    protected virtual string GenerateCode(
        Compilation compilation,
        DelegateDeclarationSyntax node,
        INamedTypeSymbol symbol,
        ImmutableArray<AttributeData> attributes,
        AnalyzerConfigOptions options
    ) => string.Empty;

    protected sealed override string GenerateCode(
        Compilation compilation,
        DelegateDeclarationSyntax node,
        ISymbol symbol,
        AttributeData attribute,
        AnalyzerConfigOptions options
    ) => GenerateCode(compilation, node, (INamedTypeSymbol)symbol, attribute, options);

    protected sealed override string GenerateCode(
        Compilation compilation,
        DelegateDeclarationSyntax node,
        ISymbol symbol,
        ImmutableArray<AttributeData> attributes,
        AnalyzerConfigOptions options
    ) => GenerateCode(compilation, node, (INamedTypeSymbol)symbol, attributes, options);
}
