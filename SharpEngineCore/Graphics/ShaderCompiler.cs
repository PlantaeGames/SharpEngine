using System.Text;

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.DirectX;

namespace SharpEngineCore.Graphics;

/// <summary>
/// Compiles d3d shader source code.
/// </summary>
internal sealed class ShaderCompiler
{
    internal readonly struct Params(D3D_FEATURE_LEVEL featureLevel,
                                       Params.Shader target)
    {
        public enum Shader
        {
            VS,
            PS
        }

        public readonly D3D_FEATURE_LEVEL FeatureLevel { get; init; } = featureLevel;
        public readonly Shader Target { get; init; } = target;
    }

    /// <summary>
    /// Compiles shader module.
    /// </summary>
    /// <param name="module">The shader module to compile</param>
    /// <param name="featureLevel">The feature level to compile the module with.</param>
    /// <returns>Blob of the compiled shader</returns>
    /// <exception cref="GraphicsException"></exception>
    public Blob Compile(ShaderModule module, ShaderCompiler.Params @params)
    {
        Blob blob = null;

        if(module.PreCompiled)
        {
            blob = new Blob(NativePreCompiled());
            return blob;
        }

        var initEnvironment = Environment.CurrentDirectory;
        Environment.CurrentDirectory = Path.GetDirectoryName(module.Name);

        try
        {
            blob = new Blob(NativeCompile());
        }
        catch
        {
            throw;
        }
        finally
        {
            Environment.CurrentDirectory = initEnvironment;
        }

        Environment.CurrentDirectory = initEnvironment;

        return blob;

        unsafe ComPtr<ID3DBlob> NativePreCompiled()
        {
            var pBlob = new ComPtr<ID3DBlob>();

            fixed (char* pName = module.Name)
            {
                GraphicsException.SetInfoQueue();
                var result = D3DReadFileToBlob(pName, pBlob.GetAddressOf());

                if(result.FAILED)
                {
                    // error here.
                    GraphicsException.ThrowLastGraphicsException(
                        "Failed to load pre compiled shader.\n" +
                        $"{module.Name}");
                }
            }

            return pBlob;
        }

        unsafe ComPtr<ID3DBlob> NativeCompile()
        {
            var bytes = module.GetBytes();

            uint flags = D3DCOMPILE.D3DCOMPILE_ENABLE_STRICTNESS;

#if DEBUG
            flags |= D3DCOMPILE.D3DCOMPILE_DEBUG;
            flags |= D3DCOMPILE.D3DCOMPILE_SKIP_OPTIMIZATION;
#endif
            string fifthVersion = "_5_0";
            string fourthVersion = "_4_0";

            string profile = @params.Target.ToString().ToLower();
            profile += @params.FeatureLevel >= D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0 ?
                                               fifthVersion : fourthVersion;

            var profileBytes = Encoding.ASCII.GetBytes(profile);

            string entryPoint = "main";
            var entryPointBytes = Encoding.ASCII.GetBytes(entryPoint);

            fixed(byte* pProfile = profileBytes)
            {
                fixed (byte* pEntry = entryPointBytes)
                {
                    var pShaderBlob = new ComPtr<ID3DBlob>();
                    var pErrorBlob = new ComPtr<ID3DBlob>();
                    fixed (byte* pData = bytes)
                    {
                        GraphicsException.SetInfoQueue();
                        var result = D3DCompile(pData, (uint)bytes.Length,
                                        (sbyte*)IntPtr.Zero,
                                        (D3D_SHADER_MACRO*)IntPtr.Zero,
                                        D3D.D3D_COMPILE_STANDARD_FILE_INCLUDE,
                                        (sbyte*)pEntry,
                                        (sbyte*)pProfile,
                                        flags,
                                        0u,
                                        pShaderBlob.GetAddressOf(),
                                        pErrorBlob.GetAddressOf());

                        if (result.FAILED || pErrorBlob.Get() != (ID3DBlob*)IntPtr.Zero)
                        {
                            var messageLength = pErrorBlob.Get()->GetBufferSize();
                            var pMessage = (byte*)pErrorBlob.Get()->GetBufferPointer();

                            var sb = new StringBuilder((int)messageLength);

                            for (var i = 0; i < (int)messageLength; i++)
                            {
                                var value = *(pMessage + (sizeof(byte) * i));

                                if (value == 0)
                                    break;

                                sb.Append(Convert.ToChar(value));
                            }

                            // error here
                            GraphicsException.ThrowLastGraphicsException(
                                $"Failed to compile shader.\nError Code: {result}\n\n" +
                                $"{sb}\n" +
                                $"Module: {module.Name}\n\n" +
                                $"Executable Path: {Environment.ProcessPath}\n" +
                                $"Working Path: {Environment.CurrentDirectory}");
                        }
                        return pShaderBlob;
                    }
                }
            }
        }
    }

    public ShaderCompiler()
    { }
}