using SharpEngineEditorControls.Controls;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace SharpEngineEditorControls.Components
{
    /// <summary>
    /// Interaction logic for HierarchyElement.xaml
    /// </summary>
    public partial class HierarchyElement : UserControl
    {
        public event Action<HierarchyElement> OnCreateNewGameObjectClicked;
        public event Action<HierarchyElement, object> OnChildGameObjectAdd;
        public event Action<HierarchyElement, object> OnRootGameObjectAdd;

        public event Action<HierarchyElement, object> OnGameObjectRemove;

        public event Action<HierarchyElement> OnClear;

        public object SelectedGameObject => (ViewItem.Header as GameObjectElement).Object;
        public TreeViewItem ViewItem => Tree.SelectedItem as TreeViewItem;

        public void Remove(TreeViewItem item)
        {
            var source = ItemsControl.ItemsControlFromItemContainer(item);
            while(source != null)
            {
                source.Items.Remove(source);

                source = source.Parent as TreeViewItem;
            }

            Tree.Items.Remove(source);
            //Tree.UpdateLayout();
        }

        public void AddRootGameObject(string name, object value)
        {
            var treeItem = new TreeViewItem();

            var gameObject = new GameObjectElement();
            gameObject.Object = value;
            gameObject.ItemObject = treeItem;

            treeItem.Header = gameObject;

            Tree.Items.Add(treeItem);
            //Tree.UpdateLayout();
            OnRootGameObjectAdd?.Invoke(this, value);
        }

        public void AddChildGameObject(TreeViewItem parent, string name, object value)
        {
            var treeItem = new TreeViewItem();

            var gameObject = new GameObjectElement();
            gameObject.Object = value;
            gameObject.ItemObject = treeItem;

            treeItem.Header = gameObject;

            parent.Items.Add(treeItem);
            //Tree.UpdateLayout();

            OnChildGameObjectAdd?.Invoke(this, value);
        }

        public void Clear()
        {
            Tree.Items.Clear();
            //Tree.UpdateLayout();

            OnClear?.Invoke(this);
        }

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
