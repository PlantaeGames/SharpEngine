using SharpEngineEditorControls.Attributes;
using SharpEngineEditorControls.Controls;
using SharpEngineEditorControls.Convertors;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SharpEngineEditorControls.Editors;

[SharpEngineEditor(typeof(Vector3))]
internal sealed class SharpEngineVector3Editor : SharpEngineEditor
{
    public override UICollection CreateUI(SharpEngineEditorResolver resolver, PrimitiveType item)
    {
        _record.Push(item);

        var collection = new UICollection();
        var panel = new DockPanel();

        var label = new Label();
        label.Content = $"{item.FieldInfo.Name}: ";
        label.HorizontalAlignment = HorizontalAlignment.Left;
        label.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        label.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

        var vector3 = new Vector3Element();
        vector3.Value = (Vector3)item.FieldInfo.GetValue(item.Parent);

        vector3.OnChange += (_, value) =>
        {
            item.FieldInfo.SetValue(item.Parent, value);
        };

        panel.Children.Add(label);
        panel.Children.Add(vector3);

        collection += panel;

        return collection;
    }
}
