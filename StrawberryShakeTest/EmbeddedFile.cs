using System.Reflection;

namespace StrawberryShakeTest;

public static class EmbeddedFile
{
    public static string GetContent(string path, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();

        path = TransformPath(path, assembly);
        if (!Exists(path, assembly))
        {
            throw new InvalidOperationException($"Embedded file doesn't exist: {path}.");
        }

        using Stream? stream = assembly.GetManifestResourceStream(path);
        using StreamReader reader = new(stream!);
        return reader.ReadToEnd();
    }

    private static bool Exists(string path, Assembly assembly)
    {
        return assembly.GetManifestResourceNames().Any(x => x == path);
    }

    private static string TransformPath(string path, Assembly assembly)
    {
        string transformedPath = path.Replace('/', '.').Replace('\\', '.');

        return $"{assembly.GetName().Name}.{transformedPath}";
    }
}
