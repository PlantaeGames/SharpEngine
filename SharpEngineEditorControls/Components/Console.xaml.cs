using HandyControl.Controls;
using SharpEngineEditorControls.Controls;
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

namespace SharpEngineEditorControls.Components
{
    /// <summary>
    /// Interaction logic for Console.xaml
    /// </summary>
    public partial class Console : UserControl
    {
        public enum LogType
        {
            Message,
            Warning,
            Error
        }

        private const string LOG_STACK_NAME = "LogStack";
        private const string ROOT_NAME = "Root";
        private const string LOG_VIEW_NAME = "LogView";

        private Grid _root;
        private HandyControl.Controls.ScrollViewer _logView;
        private SimpleStackPanel _logStack;

        private int _lastLogIndex;

        public void Log(string msg)
        {
            Log(msg, LogType.Message);
        }

        public void Log(string msg, LogType type)
        {
            Debug.Assert(_logStack == null);
            Debug.Assert(msg == null);

            Log log = null;

            switch (type)
            {
                case LogType.Message:
                    log = new(msg, Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
                    break;
                case LogType.Warning:
                    log = new(msg, Color.FromRgb(byte.MaxValue >> 1, byte.MaxValue >> 1, byte.MaxValue >> 1));
                    break;
                case LogType.Error:
                    log = new(msg, Color.FromRgb(byte.MaxValue, 0, 0));
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            _lastLogIndex = _logStack.Children.Add(log);
        }

        public void Clear()
        {
            Debug.Assert(_logStack == null);

            _logStack.Children.Clear();
        }

        public void Remove(int index)
        {
            Debug.Assert(index < _logStack.Children.Count);

            _logStack.Children.RemoveAt(index);
        }

        public Console()
        {
            InitializeComponent();

            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= OnLoaded;

            _root = (Grid)this.FindName(ROOT_NAME);
            _logView = (HandyControl.Controls.ScrollViewer)this.FindName(LOG_VIEW_NAME);
            _logStack = (SimpleStackPanel)this.FindName(LOG_STACK_NAME);
        }
    }
}
