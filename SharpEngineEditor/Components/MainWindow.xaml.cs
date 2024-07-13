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
using NetDock.Controls;
using SharpEngineCore;
using SharpEngineEditor.Extensions;
using SharpEngineEditor.Misc;
using SharpEngineEditor.Tests;

namespace SharpEngineEditor.Components
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SharpEngineEditorControls.Components.ConsoleWindow _console;
        private SharpEngineView _engineView;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _console = new SharpEngineEditorControls.Components.ConsoleWindow();
            _engineView = new SharpEngineView();

            var consoleDockItem = new DockItem(_console);
            {
                var name = _console.Name;
                consoleDockItem.Name = name;
                consoleDockItem.TabName = name;
            }

            var sharpEngineViewDockItem = new DockItem(_engineView);
            {
                var name = _engineView.Name;
                sharpEngineViewDockItem.Name = name;
                sharpEngineViewDockItem.TabName = name;
            }

            DockSurface.Add(consoleDockItem, NetDock.Enums.DockDirection.Bottom);
            DockSurface.Add(sharpEngineViewDockItem, NetDock.Enums.DockDirection.Top);
        }

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}