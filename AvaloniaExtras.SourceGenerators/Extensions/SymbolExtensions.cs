using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace AvaloniaExtras.SourceGenerators.Extensions;

internal static class SymbolExtensions
{
    public static string NamespaceOrEmpty(this ISymbol symbol) =>
        symbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : string.Join(".", symbol.ContainingNamespace.ConstituentNamespaces);

    public static string? NamespaceOrNull(this ISymbol symbol) =>
        symbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : string.Join(".", symbol.ContainingNamespace.ConstituentNamespaces);

    public static bool HasAttribute(this ISymbol symbol, string attributeName) =>
        symbol.GetAttributes().Any(x => x.AttributeClass?.Name == attributeName);

    public static bool HasAttribute<TAttribute>(this ISymbol symbol)
        where TAttribute : Attribute => HasAttribute(symbol, typeof(TAttribute).Name);

    public static AttributeData GetAttribute<TAttribute>(this ISymbol symbol)
        where TAttribute : Attribute =>
        symbol.GetAttributes().First(x => x.AttributeClass?.Name == typeof(TAttribute).Name);

    public static bool HasInterface(this ITypeSymbol typeSymbol, ITypeSymbol interfaceSymbol) =>
        HasInterface(typeSymbol, interfaceSymbol.ToDisplayString());

    public static bool HasInterface(this ITypeSymbol typeSymbol, string fullInterfaceName) =>
        typeSymbol.AllInterfaces.Any(x => x.ToDisplayString() == fullInterfaceName);

    public static string ToFullDisplayString(this ISymbol s) =>
        s.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    /// <summary>
    /// Gets the full name of the namespace, including parent namespaces.
    /// </summary>
    /// <param name="namespaceSymbol">The namespace symbol.</param>
    /// <param name="fullNamespace">Optional. The initial full name to start with.</param>
    /// <returns>The full name of the namespace.</returns>
    public static string FullNamespace(
        this INamespaceSymbol? namespaceSymbol,
        string? fullNamespace = null
    )
    {
        fullNamespace ??= string.Empty;

        if (namespaceSymbol == null)
        {
            return fullNamespace;
        }

        if (namespaceSymbol.ContainingNamespace != null)
        {
            fullNamespace = namespaceSymbol.ContainingNamespace.FullNamespace(fullNamespace);
        }

        if (!fullNamespace.Equals(string.Empty, StringComparison.OrdinalIgnoreCase))
        {
            fullNamespace += ".";
        }

        fullNamespace += namespaceSymbol.Name;

        return fullNamespace;
    }

    public static bool IsOfBaseType(this ITypeSymbol symbol, string type)
    {
        var baseType = symbol.BaseType;
        while (baseType != null)
        {
            if (baseType.Name == type)
                return true;

            baseType = baseType.BaseType;
        }

        return false;
    }

    public static bool IsOfBaseType(this ITypeSymbol? type, ITypeSymbol? baseType)
    {
        if (type is ITypeParameterSymbol p)
            return p.ConstraintTypes.Any(ct => ct.IsOfBaseType(baseType));

        while (type != null)
        {
            if (SymbolEqualityComparer.Default.Equals(type, baseType))
                return true;

            type = type.BaseType;
        }

        return false;
    }

    public static IEnumerable<INamedTypeSymbol> CollectTypeSymbols(
        this INamespaceSymbol namespaceSymbol,
        INamedTypeSymbol? targetSymbol
    )
    {
        if (targetSymbol is null)
        {
            foreach (var x1 in Enumerable.Empty<INamedTypeSymbol>())
                yield return x1;
            yield break;
        }

        foreach (
            var namedTypeSymbol in namespaceSymbol
                .GetTypeMembers()
                .Where(x => IsDerivedFrom(x, targetSymbol))
        )
            yield return namedTypeSymbol;

        // Recursively collect types from nested namespaces
        foreach (var nestedNamespace in namespaceSymbol.GetNamespaceMembers())
        foreach (var nestedTypeSymbol in nestedNamespace.CollectTypeSymbols(targetSymbol))
            yield return nestedTypeSymbol;
    }

    public static bool IsDerivedFrom(
        this INamedTypeSymbol? classSymbol,
        INamedTypeSymbol targetSymbol
    )
    {
        while (classSymbol != null)
        {
            if (SymbolEqualityComparer.Default.Equals(classSymbol.BaseType, targetSymbol))
                return true;
            classSymbol = classSymbol.BaseType;
        }

        return false;
    }

    public static bool? IsSpecialType(this ITypeSymbol? symbol)
    {
        if (symbol == null)
        {
            return null;
        }

        return symbol is IArrayTypeSymbol
            || symbol.SpecialType != SpecialType.None
            || (
                symbol.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T
                && symbol.BaseType != null
                && symbol.BaseType.SpecialType != SpecialType.None
            );
    }

    public static bool IsSpecialType(this ITypeSymbol symbol, SpecialType specialType)
    {
        return symbol.SpecialType == specialType;
    }
}
