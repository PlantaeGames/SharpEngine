using HandyControl.Controls;
using SharpEngineEditorControls.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class ConsoleElement : UserControl
    {
        public enum LogType
        {
            Message,
            Warning,
            Error
        }

        private int _lastLogIndex;

        public void Log(string msg)
        {
            Log(msg, LogType.Message);
        }

        public void Log(string msg, LogType type)
        {
            Debug.Assert(msg == null);

            Log log = null;

            switch (type)
            {
                case LogType.Message:
                    log = new($"Message: {msg}", Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
                    break;
                case LogType.Warning:
                    log = new($"Warning: {msg}", Color.FromRgb(235, 146, 52));
                    break;
                case LogType.Error:
                    log = new($"Error: {msg}", Color.FromRgb(byte.MaxValue, 0, 0));
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            _lastLogIndex = LogStack.Children.Add(log);
        }

        public void Clear()
        {
            LogStack.Children.Clear();
        }

        public void Remove(int index)
        {
            Debug.Assert(index < LogStack.Children.Count);

            LogStack.Children.RemoveAt(index);
        }

        public ConsoleElement()
        {
            InitializeComponent();

            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= OnLoaded;

           //Clear();
        }
    }
}
