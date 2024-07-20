using SharpEngineEditorControls.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using SharpEngineEditorControls.Attributes;
using System.Windows.Controls;
using SharpEngineEditorControls.Convertors;

namespace SharpEngineEditorControls.Editors
{
    [SharpEngineEditor(typeof(float))]
    public sealed class SharpEngineFloatEditor : SharpEngineEditor
    {
        public override UICollection CreateUI(SharpEngineEditorResolver resolver, PrimitiveType item)
        {
            _record.Push(item);

            var collection = new UICollection();

            var label = new Label();
            label.Content = $"{item.FieldInfo.Name}: ";
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            label.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            var numBox = new HandyControl.Controls.NumericUpDown();
            numBox.Maximum = float.MaxValue;
            numBox.Minimum = float.MinValue;

            BindingOperations.SetBinding(numBox,
                HandyControl.Controls.NumericUpDown.ValueProperty,
                new Binding(item.PropertyName)
                {
                    Converter = new DoubleToFloatConvertor(),
                    Source = item,
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            var panel = new System.Windows.Controls.DockPanel();
            panel.Children.Add(label);
            panel.Children.Add(numBox);

            collection += panel;

            return collection;
        }
    }
}