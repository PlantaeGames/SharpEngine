using HandyControl.Controls;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SharpEngineEditorControls.Components
{
    public class PropertyGridDemoModel
    {
        public class Test
        { 
            public string TestClassA { get; set; }
            public int TestClassB { get; set; }
        }

        [Category("Category1")]
        public string String { get; set; }
        [Category("Category2")]
        public int Integer { get; set; }
        [Category("Category2")]
        public bool Boolean { get; set; }
        [Category("Category1")]
        public Gender Enum { get; set; }
        public System.Windows.HorizontalAlignment HorizontalAlignment { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }
        public ImageSource ImageSource { get; set; }

        [Category("Category3")]
        public float TestClass { get; set; }
    }
    public enum Gender
    {
        Male,
        Female
    }


    /// <summary>
    /// Interaction logic for InspectorElement.xaml
    /// </summary>
    public partial class InspectorElement : System.Windows.Controls.UserControl
    {
        public PropertyGridDemoModel Modal { get; set; } = new();

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            PropertiesView.SelectedObject = Modal;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
        }

        public InspectorElement()
        {
            InitializeComponent();
        }
    }

    internal sealed class ComponentPropertyResolver : PropertyResolver
    {
        public override PropertyEditorBase CreateEditor(Type type)
        {
            return base.CreateEditor(type);
        }
    }
}