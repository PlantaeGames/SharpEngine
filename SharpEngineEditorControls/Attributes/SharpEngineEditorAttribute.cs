using System;
using System.Diagnostics;

namespace SharpEngineEditorControls.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class SharpEngineEditorAttribute : Attribute
    {
        public readonly Type TargetType;

        public SharpEngineEditorAttribute(Type targetType)
        {
            Debug.Assert(targetType != null);

            TargetType = targetType;
        }
    }
}