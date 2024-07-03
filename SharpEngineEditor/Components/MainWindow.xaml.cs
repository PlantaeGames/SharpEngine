using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using SharpEngineCore;
using SharpEngineEditor.Misc;
using SharpEngineEditor.Tests;

namespace SharpEngineEditor.Components
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void OnEngineLoaded(SharpEngineView _)
        {
            EngineView.OnEngineLoaded -= OnEngineLoaded;

            Debug.Assert(((IInternalEngineParameterizedTest<SharpEngineView>)new SharpEngineView_ENGINE_CALL_Test()).Run(EngineView));
        }

        private void OnEngineUnloaded(SharpEngineView _)
        {
            EngineView.OnEngineUnloaded -= OnEngineUnloaded;
        }

        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            EngineView.OnEngineLoaded += OnEngineLoaded;
            EngineView.OnEngineUnloaded += OnEngineUnloaded;
        }
    }
}