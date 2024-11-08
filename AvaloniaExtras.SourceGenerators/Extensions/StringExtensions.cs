using System;
using System.Linq;

namespace AvaloniaExtras.SourceGenerators.Extensions;

internal static class StringExtensions
{
    public static string Truncate(this string source, int maxChars) =>
        source.Length <= maxChars ? source : source[..maxChars];

    public static string SanitizeName(this string? name)
    {
        if (name is null || name.Length == 0)
        {
            return "";
        }

        if (InvalidFirstChar(name[0]))
        {
            name = "_" + name;
        }

        if (!name.Skip(1).Any(InvalidSubsequentChar))
        {
            return name;
        }

        Span<char> buf = stackalloc char[name.Length];
        name.AsSpan().CopyTo(buf);

        for (var i = 1; i < buf.Length; i++)
        {
            if (InvalidSubsequentChar(buf[i]))
            {
                buf[i] = '_';
            }
        }

        // Span<char>.ToString implementation checks for char type, new string(&buf[0], buf.length)
        return buf.ToString();

        static bool InvalidSubsequentChar(char ch) =>
            ch is not ('_' or >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9');

        static bool InvalidFirstChar(char ch) =>
            ch is not ('_' or >= 'A' and <= 'Z' or >= 'a' and <= 'z');
    }

    public static string RemoveNameof(this string value)
    {
        value = value ?? throw new ArgumentNullException(nameof(value));

        return value.Contains("nameof(")
            ? value[(value.LastIndexOf('.') + 1)..].TrimEnd(')', ' ')
            : value;
    }

    public static bool IsNullOfEmpty(this string source) => string.IsNullOrEmpty(source);
}
