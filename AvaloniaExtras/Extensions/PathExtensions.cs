namespace AvaloniaExtras.Extensions;

/// <summary>
///
/// </summary>
public static class PathExtensions
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="source"></param>
    /// <param name="replacementChar"></param>
    /// <returns></returns>
    public static string SanitizeFileName(this string source, char replacementChar = '_')
    {
        ArgumentNullException.ThrowIfNull(source);
        var blackList = new HashSet<char>(Path.GetInvalidFileNameChars()) { '"' }; // '"' not invalid in Linux, but causes problems
        var output = source.ToCharArray();
        for (int i = 0, ln = output.Length; i < ln; i++)
            if (blackList.Contains(output[i]))
                output[i] = replacementChar;

        return new string(output);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="path"></param>
    /// <param name="parts"></param>
    /// <returns></returns>
    public static string JoinPath(this string path, params string[] parts)
    {
        var paths = new List<string> { path };
        paths.AddRange(parts);
        return Path.Combine([.. paths]);
    }

    /// <summary>
    ///     Returns the absolute path for the specified path string.
    ///     Also searches the environment's PATH variable.
    /// </summary>
    /// <param name="fileName">The relative path string.</param>
    /// <returns>The absolute path or null if the file was not found.</returns>
    public static string? GetFullPath(this string fileName)
    {
        if (File.Exists(fileName))
            return Path.GetFullPath(fileName);

        var env = Environment.GetEnvironmentVariable("PATH");

        return env
            ?.Split(Path.PathSeparator)
            .Select(p => Path.Combine(p, fileName))
            .FirstOrDefault(File.Exists);
    }
}
