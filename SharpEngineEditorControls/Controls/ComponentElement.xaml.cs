using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SharpEngineEditorControls.Controls
{
    /// <summary>
    /// Interaction logic for ComponentElement.xaml
    /// </summary>
    public partial class ComponentElement : UserControl
    {
        public new string Name;

        public event Action<ComponentElement> OnRemove;

        public void ToggleExpend(bool expend)
        {
            ComponentNameHeader.IsExpanded = expend;
        }

        public void AddCollection(string name, UICollection uiCollection)
        {
            Debug.Assert(name != null);
            Debug.Assert(uiCollection != null);

            Name = name;

            ComponentNameHeader.Header = Name;
            FieldsStack.Children.Add(uiCollection);
        }

        public ComponentElement()
        {
            InitializeComponent();
        }

        private void OnRemoveClicked(object sender, RoutedEventArgs e)
        {
            OnRemove?.Invoke(this);
        }
    }
}
