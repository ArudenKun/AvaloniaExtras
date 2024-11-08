using Dunet;
using Microsoft.CodeAnalysis;

namespace AvaloniaExtras.SourceGenerators.Models;

[Union]
public partial record VariableSymbol
{
    partial record Property(IPropertySymbol PropertySymbol);

    partial record Field(IFieldSymbol FieldSymbol);
}
