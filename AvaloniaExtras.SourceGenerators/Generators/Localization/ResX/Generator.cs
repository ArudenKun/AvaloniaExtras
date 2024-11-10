using System.Collections.Generic;
using AvaloniaExtras.Attributes;
using AvaloniaExtras.SourceGenerators.Abstractions;
using AvaloniaExtras.SourceGenerators.Extensions;
using CodeGenHelpers;
using H;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AvaloniaExtras.SourceGenerators.Generators.Localization.ResX;

[Generator]
public sealed class Generator : SourceGeneratorForTypeWithAttribute<ResXLocalizerAttribute>
{
    protected override string Id => "RXLG";

    protected override IEnumerable<FileWithName> StaticSources =>
        [
            new(
                $"{typeof(ResXLocalizerAttribute).FullName}",
                Resources.ResXLocalizerAttribute_cs.AsString()
            ),
        ];

    protected override string GenerateCode(
        Compilation compilation,
        TypeDeclarationSyntax node,
        INamedTypeSymbol symbol,
        AttributeData attribute,
        AnalyzerConfigOptions options
    )
    {
        if (attribute.ConstructorArguments[0].Value is not INamedTypeSymbol resourceSymbol)
        {
            return string.Empty;
        }

        var builder = CodeBuilder.Create(symbol);

        builder.SetBaseClass("AvaloniaExtras.Localization.Abstractions.BaseLocalizer");

        builder
            .AddMethod("Reload")
            .Override()
            .MakePublicMethod()
            .WithInheritDoc()
            .WithBody(writer =>
            {
                writer.AppendLine($"{resourceSymbol.ToFullDisplayString()}.Culture = Language;");
            });

        builder
            .AddMethod("SetLanguage")
            .MakeProtectedMethod()
            .Override()
            .AddParameter("System.Globalization.CultureInfo", "language")
            .WithInheritDoc()
            .WithBody(writer =>
            {
                writer.AppendLine("base.SetLanguage(language);");
                writer.AppendLine("Reload();");
                writer.AppendLine("RefreshUi();");
            });

        builder
            .AddMethod("Get")
            .MakePublicMethod()
            .Override()
            .WithInheritDoc()
            .WithReturnType("string")
            .AddParameter("string", "key")
            .WithBody(writer =>
            {
                writer
                    .If("!HasLoaded")
                    .WithBody(ifWriter =>
                    {
                        ifWriter.AppendLine("Reload();");
                    });

                writer.AppendLine(
                    $"var langString = {resourceSymbol.ToFullDisplayString()}.ResourceManager.GetString(key, {resourceSymbol.ToFullDisplayString()}.Culture);"
                );
                writer.AppendLine(
                    "return langString != null ? langString.Replace(\"\\\\n\", \"\\n\") : $\"{Language}:{key}\";"
                );
            });

        return builder.Build();
    }
}
