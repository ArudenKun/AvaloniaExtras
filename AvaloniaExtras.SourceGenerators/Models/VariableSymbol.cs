using Microsoft.CodeAnalysis;

namespace AvaloniaExtras.SourceGenerators.Models;

public sealed record VariableSymbol
{
    public ISymbol Value { get; }

    public VariableSymbol(IPropertySymbol value)
    {
        Value = value;
    }

    public VariableSymbol(IFieldSymbol value)
    {
        Value = value;
    }

    public static IPropertySymbol ToIPropertySymbol(VariableSymbol symbol) =>
        (IPropertySymbol)symbol.Value;

    public static VariableSymbol FromIPropertySymbol(IPropertySymbol symbol) => new(symbol);

    public static IFieldSymbol ToIFieldSymbol(VariableSymbol symbol) => (IFieldSymbol)symbol.Value;

    public static VariableSymbol FromIFieldSymbol(IFieldSymbol symbol) => new(symbol);
}
