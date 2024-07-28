using SharpEngineEditorControls.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using SharpEngineEditorControls.Attributes;
using System.Windows.Controls;

namespace SharpEngineEditorControls.Editors
{
    [SharpEngineEditor(typeof(string))]
    public sealed class SharpEngineStringEditor :
        SharpEngineEditor
    {
        public override UICollection CreateUI(SharpEngineEditorResolver resolver,
            PrimitiveBinding binding)
        {
            _record.Push(binding);

            var collection = new UICollection();

            var label = new Label();
            label.Content = $"{binding.Name} : ";
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            label.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            var textBox = new HandyControl.Controls.TextBox();
            textBox.TextWrapping = TextWrapping.Wrap;

            binding.OnRefresh += x =>
            {
                textBox.Text = (string)x.Value;
            };

            textBox.TextChanged += (source, __) =>
            {
                binding.UpdateByUI(textBox.Text);
            };

            binding.Refresh();

            var panel = new System.Windows.Controls.DockPanel();
            panel.Children.Add(label);
            panel.Children.Add(textBox);

            collection += panel;

            return collection;
        }
    }
}