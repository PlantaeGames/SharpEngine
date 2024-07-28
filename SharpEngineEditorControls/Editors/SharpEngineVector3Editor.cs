using SharpEngineEditorControls.Attributes;
using SharpEngineEditorControls.Controls;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace SharpEngineEditorControls.Editors;

[SharpEngineEditor(typeof(Vector3))]
internal sealed class SharpEngineVector3Editor :
    SharpEngineEditor
{
    public override UICollection CreateUI(SharpEngineEditorResolver resolver, 
        PrimitiveBinding binding)
    {
        _record.Push(binding);

        var collection = new UICollection();
        var panel = new DockPanel();

        var label = new Label();
        label.Content = $"{binding.Name}: ";
        label.HorizontalAlignment = HorizontalAlignment.Left;
        label.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        label.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

        var vector3 = new Vector3Element();

        binding.OnRefresh += x =>
        {
            vector3.Value = (Vector3)x.Value;
        };

        vector3.OnChange += (_, value) =>
        {
            binding.UpdateByUI(value);
        };
        binding.Refresh();

        panel.Children.Add(label);
        panel.Children.Add(vector3);

        collection += panel;

        return collection;
    }
}
