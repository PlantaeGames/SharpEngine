using SharpEngineEditorControls.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using SharpEngineEditorControls.Attributes;
using System.Windows.Controls;

namespace SharpEngineEditorControls.Editors
{
    [SharpEngineEditor(typeof(string))]
    public sealed class SharpEngineStringEditor : SharpEngineEditor
    {
        public override UICollection CreateUI(SharpEngineEditorResolver resolver, PrimitiveType item)
        {
            _record.Push(item);

            var collection = new UICollection();

            var label = new Label();
            label.Content = $"{item.FieldInfo.Name} : ";
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            label.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            var textBox = new HandyControl.Controls.TextBox();
            textBox.TextWrapping = TextWrapping.Wrap;

            BindingOperations.SetBinding(textBox,
                TextBox.TextProperty,
                new Binding($"Value")
                {
                    Converter = null,
                    Source = item,
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            var panel = new System.Windows.Controls.DockPanel();
            panel.Children.Add(label);
            panel.Children.Add(textBox);

            collection += panel;

            return collection;
        }
    }
}