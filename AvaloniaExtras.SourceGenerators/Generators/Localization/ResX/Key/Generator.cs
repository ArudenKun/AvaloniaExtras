using System;
using System.Linq;
using AvaloniaExtras.Attributes;
using AvaloniaExtras.SourceGenerators.Abstractions;
using CodeGenHelpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AvaloniaExtras.SourceGenerators.Generators.Localization.ResX.Key;

[Generator]
public sealed class Generator : SourceGeneratorForTypeWithAttribute<ResXLocalizerAttribute>
{
    protected override string Id => "RXLKG";

    private static readonly string[] Exceptions =
    [
        "resourceMan",
        "resourceCulture",
        ".ctor",
        "ResourceManager",
        "Culture",
    ];

    protected override string GenerateCode(
        Compilation compilation,
        TypeDeclarationSyntax node,
        INamedTypeSymbol symbol,
        AttributeData attribute,
        AnalyzerConfigOptions options
    )
    {
        var resourceSymbol = attribute.ConstructorArguments[0].Value as INamedTypeSymbol;
        var names = resourceSymbol?.MemberNames.Except(Exceptions).ToList();

        if (resourceSymbol is null || names is null)
        {
            throw new Exception("Resource not found");
        }

        var builder = CodeBuilder
            .Create(symbol.ContainingNamespace)
            .AddClass($"{symbol.Name}Keys")
            .MakePublicClass()
            .MakeStaticClass();

        foreach (var name in names)
        {
            builder
                .AddProperty(name, Accessibility.Public)
                .SetType<string>()
                .MakeStatic()
                .WithSummary(name)
                .WithReadonlyValue($"nameof({name})");
        }

        return builder.Build();
    }
}
