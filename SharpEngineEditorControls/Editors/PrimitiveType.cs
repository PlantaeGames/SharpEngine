using System;
using System.Reflection;

namespace SharpEngineEditorControls.Editors;

public sealed class PrimitiveBinding
{
    private readonly FieldInfo _fieldInfo;
    private readonly object _parent;

    public string Name => _fieldInfo.Name;
    public object Parent => _parent;

    public bool Valid => _fieldInfo != null;

#nullable enable
    public object? Value => _fieldInfo?.GetValue(_parent);
#nullable disable

    public event Action<PrimitiveBinding> OnRefresh;

    public void Refresh()
    {
        OnRefresh?.Invoke(this);
    }

    public void UpdateByUI(object value)
    {
        _fieldInfo?.SetValue(_parent, value);
    }

#nullable enable
    public PrimitiveBinding(object parent, FieldInfo? fieldInfo)
    {
        _parent = parent;
        _fieldInfo = fieldInfo;
    }
#nullable disable
}