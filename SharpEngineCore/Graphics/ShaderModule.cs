using System.Diagnostics;

namespace SharpEngineCore.Graphics;

/// <summary>
/// Loads the shader source file.
/// </summary>
public sealed class ShaderModule
{
    private byte[] _bytes;
    public byte[] GetBytes() => _bytes;

    public ShaderModule(string path)
    {
        Debug.Assert(path != null && path != string.Empty,
            "Path can't be null here.");

        Load(path);
    }

    /// <summary>
    /// Loads the binary of the shader source file.
    /// </summary>
    /// <param name="path">Path of the shader file.</param>
    /// <exception cref="Exception"></exception>
    private void Load(string path)
    {
        try
        {
            using var s = File.OpenRead(path);
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
