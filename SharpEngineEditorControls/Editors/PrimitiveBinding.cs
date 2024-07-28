using System;
using System.Reflection;
using System.Threading;

namespace SharpEngineEditorControls.Editors;

public sealed class PrimitiveBinding
{
    private readonly FieldInfo _fieldInfo;
    private readonly object _parent;

    public string Name => _fieldInfo.Name;
    public object Parent => _parent;

    public bool Valid => _fieldInfo != null;

    public object Lock { get; private set; }

#nullable enable
    public object? Value => _fieldInfo?.GetValue(_parent);
#nullable disable

    public event Action<PrimitiveBinding> OnRefresh;

    private bool _refreshing;

    public void Refresh()
    {
        _refreshing = true;

        OnRefresh?.Invoke(this);

        _refreshing = false;
    }

    public void UpdateByUI(object value)
    {
        if(_refreshing) 
            return;

        lock (Lock)
        {
            _fieldInfo?.SetValue(_parent, value);
        }
    }

#nullable enable
    public PrimitiveBinding(object parent, FieldInfo? fieldInfo,
        object @lock)
    {
        _parent = parent;
        _fieldInfo = fieldInfo;

        Lock = @lock;
    }
#nullable disable
}