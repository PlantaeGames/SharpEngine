using SharpEngineEditorControls.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using SharpEngineEditorControls.Attributes;
using System.Windows.Controls;
using SharpEngineEditorControls.Convertors;
using System;

namespace SharpEngineEditorControls.Editors
{
    [SharpEngineEditor(typeof(int))]
    public sealed class SharpEngineIntEditor :
        SharpEngineEditor
    {
        public override UICollection CreateUI(SharpEngineEditorResolver resolver,
            PrimitiveBinding binding)
        {

            _record.Push(binding);

            var collection = new UICollection();

            var label = new Label();
            label.Content = $"{binding.Name}: ";
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            label.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            var numBox = new HandyControl.Controls.NumericUpDown();
            numBox.Maximum = int.MaxValue;
            numBox.Minimum = int.MinValue;

            binding.OnRefresh += x =>
            {
                numBox.Value = Convert.ToDouble(binding.Value);
            };

            numBox.ValueChanged += (source, _) =>
            {
                binding.UpdateByUI((int)numBox.Value);
            };

            binding.Refresh();

            var panel = new System.Windows.Controls.DockPanel();
            panel.Children.Add(label);
            panel.Children.Add(numBox);

            collection += panel;

            return collection;
        }
    }
}