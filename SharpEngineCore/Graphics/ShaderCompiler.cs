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
        return new Blob(NativeCompile());

        unsafe ComPtr<ID3DBlob> NativeCompile()
        {
            var bytes = module.GetBytes();

            uint flags = D3DCOMPILE.D3DCOMPILE_ENABLE_STRICTNESS;

#if DEBUG
            flags |= D3DCOMPILE.D3DCOMPILE_DEBUG;
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
                                        (ID3DInclude*)IntPtr.Zero,
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
                                $"{sb}");
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