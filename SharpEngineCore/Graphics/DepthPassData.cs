using System.Collections;
using System.Diagnostics;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class DepthPassData :
    IEnumerable<(Guid id, Texture2D texture, List<DepthStencilView> views, bool active)>
{
    public readonly List<Texture2D> DepthTextures = new();
    private readonly List<List<DepthStencilView>> _depthViews = new();
    public readonly List<ShaderResourceView> ShaderViews = new();
    public readonly List<Sampler> DepthSamplers = new();

    private readonly List<Guid> _ids = new();
    private readonly List<bool> _actives = new();

    private readonly int _initSize;
    public bool SamplersInitialized { get; private set; }

    public void SetSamplers(Sampler[] samplers)
    {
        Debug.Assert(!SamplersInitialized,
            "Samplers already Initilized");
        Debug.Assert(samplers.Length == _initSize,
            "Mismatch samplers used.");

        for(var i = 0; i < _initSize; i++)
        {
            DepthSamplers.Add(samplers[i]);
        }

        SamplersInitialized = true;
    }

    public void Add(Guid id, Texture2D texture, List<DepthStencilView> views,
        ShaderResourceView shaderView)
    {
        Debug.Assert(SamplersInitialized);

        for(var i = 0; i < DepthTextures.Count; i++)
        {
            if (DepthTextures[i].IsValid())
                continue;

            _ids[i] = id;
            DepthTextures[i] = texture;
            _depthViews[i] = views;
            ShaderViews[i] = shaderView;

            _actives[i] = true;

            return;
        }

        _ids.Add(id);
        DepthTextures.Add(texture);
        _depthViews.Add(views);
        ShaderViews.Add(shaderView);

        _actives.Add(true);
    }

    public (Texture2D texture, List<DepthStencilView> views, bool active) Get(Guid id)
    {
        Debug.Assert(SamplersInitialized);

        var index = _ids.IndexOf(id);
        return (DepthTextures[index], _depthViews[index], _actives[index]);
    }

    public void Pause(Guid id)
    {
        Debug.Assert(SamplersInitialized);

        var index = _ids.IndexOf(id);
        _actives[index] = false;
    }

    public void Resume(Guid id)
    {
        Debug.Assert(SamplersInitialized);

        var index = _ids.IndexOf(id);
        _actives[index] = true;
    }

    public void Remove(Guid id)
    {
        Debug.Assert(SamplersInitialized);

        var index = _ids.IndexOf(id);

        if (DepthTextures.Count == _initSize)
        {
            _ids[index] = Guid.Empty;
            DepthTextures[index] = Texture2D.GetPlaceHolder();
            _depthViews[index] = new List<DepthStencilView>() 
                                { new DepthStencilView(null, null, new(), null)};
            ShaderViews[index] = new(null, null, new(), null);

            _actives[index] = false;

            return;
        }

        DepthTextures.RemoveAt(index);
        _depthViews.RemoveAt(index);
        ShaderViews.RemoveAt(index);
        DepthSamplers.RemoveAt(index);

        _actives.RemoveAt(index);
    }

    public IEnumerator<(Guid id, Texture2D texture, List<DepthStencilView> views, bool active)>
        GetEnumerator()
    {
        Debug.Assert(SamplersInitialized);

        for (var i = 0; i < _ids.Count; i++)
        {
            yield return (
                _ids[i],
                DepthTextures[i],
                _depthViews[i],
                _actives[i]
                );
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public DepthPassData(int size)
    {
        _initSize = size;

        for(var i = 0; i < _initSize; i++)
        {
            _ids.Add(Guid.Empty);
            DepthTextures.Add(Texture2D.GetPlaceHolder());
            _depthViews.Add(new() { new DepthStencilView(null, null, new(), null)});
            ShaderViews.Add(new ShaderResourceView(null, null, new(), null));

            _actives.Add(false);
        }
    }
}
