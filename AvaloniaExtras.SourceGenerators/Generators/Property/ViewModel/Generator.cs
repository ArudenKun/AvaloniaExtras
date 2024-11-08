using System;
using System.Threading;
using AvaloniaExtras.SourceGenerators.Extensions;
using CodeGenHelpers;
using H.Generators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AvaloniaExtras.SourceGenerators.Generators.Property.ViewModel;

[Generator]
public sealed class Generator : IIncrementalGenerator
{
    private const string Id = "PVG";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context
            .SyntaxProvider.CreateSyntaxProvider(
                IsSyntaxTarget,
                (syntaxContext, _) => syntaxContext
            )
            .Combine(context.CompilationProvider)
            .Combine(context.AnalyzerConfigOptionsProvider)
            .SelectAndReportExceptions(
                (tuple, _) => Generate(tuple.Left.Left, tuple.Left.Right, tuple.Right),
                context,
                Id
            )
            .AddSource(context);
    }

    private static bool IsSyntaxTarget(SyntaxNode node, CancellationToken ct)
    {
        if (node is ClassDeclarationSyntax baseTypeDeclarationSyntax)
        {
            return baseTypeDeclarationSyntax.Identifier.Text.EndsWith("ViewModel");
        }

        return false;
    }

    private static FileWithName Generate(
        GeneratorSyntaxContext context,
        Compilation compilation,
        AnalyzerConfigOptionsProvider optionsProvider
    )
    {
        var constants =
            optionsProvider.GetGlobalOption("DefineConstants", prefix: "RecognizeFramework")
            ?? string.Empty;

        if (!constants.Contains("ENABLE_VIEWMODEL_GENERATOR"))
        {
            return FileWithName.Empty;
        }

        var viewModelSymbol = context.SemanticModel.GetDeclaredSymbol(context.Node);

        if (viewModelSymbol is null)
        {
            return FileWithName.Empty;
        }

        var viewSymbol = GetView(compilation, viewModelSymbol);
        if (viewSymbol is null)
            return FileWithName.Empty;

        var builder = CodeBuilder.Create(viewSymbol);

        builder
            .AddProperty("ViewModel")
            .SetType((INamedTypeSymbol)viewModelSymbol)
            .WithAccessModifier(Accessibility.Public)
            .WithGetterExpression(
                $"DataContext as {viewModelSymbol.ToFullDisplayString()} ?? throw new global::System.ArgumentNullException(nameof(DataContext))"
            )
            .WithSetterExpression("DataContext = value");

        return new FileWithName(
            $"{viewSymbol.ToDisplayString()}.Property.ViewModel.g.cs",
            builder.Build()
        );
    }

    private static INamedTypeSymbol? GetView(Compilation compilation, ISymbol? symbol)
    {
        if (symbol is null)
        {
            return null;
        }

        var viewName = symbol.ToDisplayString().Replace("ViewModel", "View");
        var viewSymbol = compilation.GetTypeByMetadataName(viewName);

        if (viewSymbol is not null)
            return viewSymbol;

        viewName = symbol.ToDisplayString().Replace(".ViewModels.", ".Views.");
        viewName = viewName.Remove(viewName.IndexOf("ViewModel", StringComparison.Ordinal));
        return compilation.GetTypeByMetadataName(viewName);
    }
}
