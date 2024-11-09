using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AvaloniaExtras.SourceGenerators.Abstractions;

public abstract class SourceGeneratorForMethodWithAttribute<TAttribute>
    : SourceGeneratorForMemberWithAttribute<TAttribute, MethodDeclarationSyntax>
    where TAttribute : Attribute
{
    protected abstract string GenerateCode(
        Compilation compilation,
        MethodDeclarationSyntax node,
        IMethodSymbol symbol,
        AttributeData attribute,
        AnalyzerConfigOptions options
    );

    protected sealed override string GenerateCode(
        Compilation compilation,
        MethodDeclarationSyntax node,
        ISymbol symbol,
        AttributeData attribute,
        AnalyzerConfigOptions options
    ) => GenerateCode(compilation, node, (IMethodSymbol)symbol, attribute, options);
}
