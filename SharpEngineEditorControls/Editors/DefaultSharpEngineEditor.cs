using SharpEngineEditorControls.Controls;

namespace SharpEngineEditorControls.Editors
{
    public sealed class DefaultSharpEngineEditor : SharpEngineEditor
    {
        public override UICollection CreateUI(SharpEngineEditorResolver resolver, PrimitiveType item)
        {
            return base.CreateUI(resolver, item);
        }
    }
}