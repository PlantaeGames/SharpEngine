using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for AddComponentElement.xaml
    /// </summary>
    public partial class AddComponentElement : UserControl
    {
        public event Action<AddComponentElement, string> OnAddClicked;

        public AddComponentElement()
        {
            InitializeComponent();

            AddButton.Click += OnAddButtonClicked;
        }

        private void OnAddButtonClicked(object sender, RoutedEventArgs e)
        {
            OnAddClicked?.Invoke(this, ComponentName.Text);
        }
    }
}
