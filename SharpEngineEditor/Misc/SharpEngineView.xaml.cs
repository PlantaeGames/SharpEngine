using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using TerraFX.Interop.DirectX;

namespace SharpEngineEditor.Misc
{
    /// <summary>
    /// Interaction logic for SharpEngineView.xaml
    /// </summary>
    public partial class SharpEngineView : UserControl
    {
#nullable disable
        private SharpEngineHost _host;
#nullable enable

        public SharpEngineView()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            _host = new SharpEngineHost();
            Content = _host;
        }
    }
}
