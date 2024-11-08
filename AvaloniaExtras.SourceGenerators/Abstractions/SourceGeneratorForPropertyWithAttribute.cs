using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AvaloniaExtras.SourceGenerators.Abstractions;

public abstract class SourceGeneratorForPropertyWithAttribute<TAttribute>
    : SourceGeneratorForMemberWithAttribute<TAttribute, PropertyDeclarationSyntax>
    where TAttribute : Attribute
{
    protected abstract string GenerateCode(
        Compilation compilation,
        PropertyDeclarationSyntax node,
        IPropertySymbol symbol,
        TAttribute attribute,
        AnalyzerConfigOptions options
    );

    protected sealed override string GenerateCode(
        Compilation compilation,
        PropertyDeclarationSyntax node,
        ISymbol symbol,
        TAttribute attribute,
        AnalyzerConfigOptions options
    ) => GenerateCode(compilation, node, (IPropertySymbol)symbol, attribute, options);
}
