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

namespace SharpEngineEditorControls.Components
{
    /// <summary>
    /// Interaction logic for HierarchyElement.xaml
    /// </summary>
    public partial class HierarchyElement : UserControl
    {
        public event Action<HierarchyElement> OnCreateNewGameObjectClicked;

        public HierarchyElement()
        {
            InitializeComponent();

            ContextMenu.Opened += OnContextMenuOpened;
            ContextMenu.Closed += OnContextMenuClosed;

            CreateButton.Click += OnCreateButtonClicked; ;
        }

        private void OnCreateButtonClicked(object sender, RoutedEventArgs e)
        {
            OnCreateNewGameObjectClicked?.Invoke(this);
        }

        private void OnContextMenuClosed(object sender, RoutedEventArgs e)
        {}

        private void OnContextMenuOpened(object sender, RoutedEventArgs e)
        {}

        private void ContextMenuCreateNewGameObjectClicked(object sender, RoutedEventArgs e)
        {
            OnCreateNewGameObjectClicked?.Invoke(this);
        }
    }
}
