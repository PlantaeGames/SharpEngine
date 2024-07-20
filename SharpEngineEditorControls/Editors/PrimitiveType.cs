using System;
using System.Windows;
using System.Reflection;

namespace SharpEngineEditorControls.Editors
{
    public sealed class PrimitiveType : DependencyObject
    {
        private readonly FieldInfo _fieldInfo;
        private readonly object _parent;

        public object Parent => _parent;
        public Type Type => _fieldInfo.FieldType;
        public FieldInfo FieldInfo => _fieldInfo;

        public string PropertyName => nameof(Value);

        public object Value
        {
            get => GetValue(ValueProperty);
            set
            {
                SetValue(ValueProperty, value);
                _fieldInfo?.SetValue(_parent, Value);
            }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(PrimitiveType));

#nullable enable
        public PrimitiveType(object parent, FieldInfo? fieldInfo)
        {
            _parent = parent;
            _fieldInfo = fieldInfo;

            Value = _fieldInfo?.GetValue(_parent);
        }
#nullable disable
    }
}