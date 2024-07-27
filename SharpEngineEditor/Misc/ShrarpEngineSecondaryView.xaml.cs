using SharpEngineCore.Graphics;
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

namespace SharpEngineEditor.Misc
{
    /// <summary>
    /// Interaction logic for ShrarpEngineSecondaryView.xaml
    /// </summary>
    public partial class SharpEngineSecondaryView : UserControl
    {
        private SharpEngineSecnodaryHost _host;

        public event Action<SharpEngineSecondaryView> OnWindowCreated;
        public event Action<SharpEngineSecondaryView> OnWindowDestroyed;

#nullable enable
        public SecondaryWindow? SecondaryWindow => _host.SecondaryWindow;
#nullable disable
        public SharpEngineSecondaryView()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            _host = new();
            _host.OnBuildCore += OnWindowLoaded;
            _host.OnDestoryCore += OnWindowUnloaded;

            this.Content = _host;

            void OnWindowLoaded(SharpEngineSecnodaryHost host, SecondaryWindow window)
            {
                _host.OnBuildCore -= OnWindowLoaded;

                OnWindowCreated?.Invoke(this);
            }

            void OnWindowUnloaded(SharpEngineSecnodaryHost host, SecondaryWindow window)
            {
                _host.OnDestoryCore -= OnWindowUnloaded;

                OnWindowDestroyed?.Invoke(this);
            }
        }
    }
}
