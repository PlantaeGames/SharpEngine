using System.Diagnostics;
using System.Runtime.CompilerServices;
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
using HandyControl.Tools;
using NetDock;
using NetDock.Controls;
using SharpEngineCore;
using SharpEngineCore.ECS;
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
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}