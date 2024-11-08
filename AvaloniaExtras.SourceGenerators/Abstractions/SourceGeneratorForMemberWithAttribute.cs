using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using AvaloniaExtras.SourceGenerators.Extensions;
using H.Generators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AvaloniaExtras.SourceGenerators.Abstractions;

public abstract class SourceGeneratorForMemberWithAttribute<TAttribute, TDeclarationSyntax>
    : IIncrementalGenerator
    where TAttribute : Attribute
    where TDeclarationSyntax : SyntaxNode
{
    protected const string Space = " ";
    protected const string Comma = ",";
    protected const string Tab = "\t";

    protected abstract string Id { get; }

    protected virtual IEnumerable<FileWithName> StaticSources => [];

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        foreach (var (name, source) in StaticSources)
        {
            context.RegisterPostInitializationOutput(x => x.AddSource($"{name}.g.cs", source));
        }

        var syntaxProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
            typeof(TAttribute).FullName!,
            IsSyntaxTarget,
            GetSyntaxTarget
        );

        context
            .CompilationProvider.Combine(syntaxProvider.Collect())
            .Combine(context.AnalyzerConfigOptionsProvider)
            .SelectAndReportExceptions(
                (tuple, ct) => OnExecute(tuple.Left.Left, tuple.Left.Right, tuple.Right, ct),
                context,
                Id
            )
            .AddSource(context);
    }

    protected virtual bool IsSyntaxTarget(SyntaxNode node, CancellationToken _) =>
        node is TDeclarationSyntax;

    private static GeneratorAttributeSyntaxContext GetSyntaxTarget(
        GeneratorAttributeSyntaxContext context,
        CancellationToken _
    ) => context;

    private IEnumerable<FileWithName> OnExecute(
        Compilation compilation,
        ImmutableArray<GeneratorAttributeSyntaxContext> generatorAttributeSyntaxContexts,
        AnalyzerConfigOptionsProvider options,
        CancellationToken cancellationToken
    )
    {
        foreach (var generatorAttributeSyntaxContext in generatorAttributeSyntaxContexts)
        {
            if (cancellationToken.IsCancellationRequested)
                continue;

            var node = generatorAttributeSyntaxContext.TargetNode;
            var symbol = generatorAttributeSyntaxContext.TargetSymbol;
            var attributes = generatorAttributeSyntaxContext.Attributes;

            string generatedCode;
            if (attributes.Length > 1)
            {
                generatedCode = GenerateCode(
                    compilation,
                    (TDeclarationSyntax)node,
                    symbol,
                    [.. attributes.Select(x => x.MapToAttributeType<TAttribute>())],
                    options.GlobalOptions
                );
            }
            else
            {
                generatedCode = GenerateCode(
                    compilation,
                    (TDeclarationSyntax)node,
                    symbol,
                    attributes.First().MapToAttributeType<TAttribute>(),
                    options.GlobalOptions
                );
            }

            if (generatedCode.IsNullOfEmpty())
            {
                continue;
            }

            yield return new FileWithName(GenerateFilename(symbol), generatedCode);
        }
    }

    protected abstract string GenerateCode(
        Compilation compilation,
        TDeclarationSyntax node,
        ISymbol symbol,
        TAttribute attribute,
        AnalyzerConfigOptions options
    );

    protected abstract string GenerateCode(
        Compilation compilation,
        TDeclarationSyntax node,
        ISymbol symbol,
        ImmutableArray<TAttribute> attributes,
        AnalyzerConfigOptions options
    );

    private const string Ext = ".g.cs";
    private const int MaxFileLength = 255;

    protected virtual string GenerateFilename(ISymbol symbol)
    {
        var gn = $"{Format()}{Ext}";
        return gn;

        string Format() =>
            string.Join("_", $"{symbol}".Split(Path.GetInvalidPathChars()))
                .Truncate(MaxFileLength - Ext.Length);
    }
}
