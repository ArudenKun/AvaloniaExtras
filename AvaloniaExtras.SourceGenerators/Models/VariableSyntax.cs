using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AvaloniaExtras.SourceGenerators.Models;

public sealed class VariableSyntax
{
    public SyntaxNode Value { get; }

    public VariableSyntax(PropertyDeclarationSyntax value)
    {
        Value = value;
    }

    public VariableSyntax(FieldDeclarationSyntax value)
    {
        Value = value;
    }

    public static implicit operator PropertyDeclarationSyntax(VariableSyntax variableSyntax) =>
        (PropertyDeclarationSyntax)variableSyntax.Value;

    public static implicit operator VariableSyntax(PropertyDeclarationSyntax propertySyntax) =>
        new(propertySyntax);

    public static implicit operator FieldDeclarationSyntax(VariableSyntax variableSyntax) =>
        (FieldDeclarationSyntax)variableSyntax.Value;

    public static implicit operator VariableSyntax(FieldDeclarationSyntax fieldSyntax) =>
        new(fieldSyntax);
}
