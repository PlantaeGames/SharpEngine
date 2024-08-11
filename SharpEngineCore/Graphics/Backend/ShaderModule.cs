using System.Diagnostics;

namespace SharpEngineCore.Graphics;

/// <summary>
/// Loads the shader source file.
/// </summary>
public sealed class ShaderModule
{
    public readonly string Name;
    public readonly bool PreCompiled = false;
    private byte[] _bytes;
    public byte[] GetBytes() => _bytes;

    public ShaderModule(string path, bool preCompiled = false)
    {
        Debug.Assert(path != null && path != string.Empty,
            "Path can't be null here.");

        Name = path;
        PreCompiled = preCompiled;

        Load();
    }

    /// <summary>
    /// Loads the binary of the shader source file.
    /// </summary>
    /// <exception cref="Exception"></exception>
    private void Load()
    {
        if (PreCompiled)
            return;

        try
        {
            using var s = File.OpenRead(Name);
            using BinaryReader br = new(s);

            var end = br.BaseStream.Seek(0, SeekOrigin.End);
            br.BaseStream.Seek(0, SeekOrigin.Begin);

            Debug.Assert(end > 0,
                "File can't be invalid or empty.");

            _bytes = br.ReadBytes((int)end);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
