using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using TerraFX.Interop.Windows;

namespace SharpEngineEditor.Misc
{
    /// <summary>
    /// Interaction logic for SharpEngineView.xaml
    /// </summary>
    public partial class SharpEngineView : UserControl
    {
#nullable disable
        public event Action<SharpEngineView> OnEngineLoaded;
        public event Action<SharpEngineView> OnEngineUnloaded;

        public Assembly EngineCoreAssembly => _host.EngineCoreAssembly;
#nullable enable
        public GameAssembly? GameAssembly => _host.GameAssembly;
#nullable disable

        private SharpEngineHost _host;
#nullable enable

        public void ENGINE_CALL(Action action)
        {
            _host.ENGINE_ACTION_CALL(action);
        }

        public T ENGINE_CALL<T>(Func<T> function)
        {
            return _host.ENGINE_FUNC_CALL(function);
        }

        public SharpEngineView()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            _host = new SharpEngineHost();

            _host.OnEngineLoaded += OnHostEngineLoaded;
            _host.OnEngineUnloaded += OnHostEngineUnloaded;

            Content = _host;

            void OnHostEngineLoaded(SharpEngineHost _)
            {
                _host.OnEngineLoaded -= OnHostEngineLoaded;

                OnEngineLoaded?.Invoke(this);
            }

            void OnHostEngineUnloaded(SharpEngineHost _)
            {
                _host.OnEngineUnloaded -= OnHostEngineUnloaded;

                OnEngineUnloaded?.Invoke(this);
            }
        }
    }
}
