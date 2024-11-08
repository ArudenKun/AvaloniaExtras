using Dunet;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AvaloniaExtras.SourceGenerators.Models;

[Union]
public partial record VariableSyntax
{
    partial record Property(PropertyDeclarationSyntax PropertyDeclarationSyntax);

    partial record Field(FieldDeclarationSyntax FieldDeclarationSyntax);
}
