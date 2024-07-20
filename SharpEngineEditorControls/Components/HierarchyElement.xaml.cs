using SharpEngineEditorControls.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
#nullable enable
        public event Action<HierarchyElement, object?> OnSelectedChanged;
#nullable disable

        public event Action<HierarchyElement, object> OnGameObjectRemove;

        public event Action<HierarchyElement> OnClear;

#nullable enable
        public object? SelectedGameObject => (ViewItem?.Header as GameObjectElement)?.Object;
#nullable disable
        public TreeViewItem ViewItem => Tree.SelectedItem as TreeViewItem;

        public void Remove(TreeViewItem item)
        {
            var parent = item.Parent as TreeViewItem;
            if (parent != null)
            {
                parent.Items.Remove(item);
            }
            else
            {
                Tree.Items.Remove(item);
            }

            OnGameObjectRemove?.Invoke(this, ((GameObjectElement)item.Header).Object);
        }

        public void AddRootGameObject(string name, object value)
        {
            var treeItem = new TreeViewItem();

            var gameObject = new GameObjectElement();
            gameObject.Object = value;
            gameObject.ItemObject = treeItem;
            gameObject.OnDeleteClicked += (x) =>
            {
                Remove(x.ItemObject);
            };

            treeItem.Header = gameObject;

            Tree.Items.Add(treeItem);
            OnRootGameObjectAdd?.Invoke(this, value);
        }

        public void AddChildGameObject(TreeViewItem parent, string name, object value)
        {
            var treeItem = new TreeViewItem();

            var gameObject = new GameObjectElement();
            gameObject.Object = value;
            gameObject.ItemObject = treeItem;
            gameObject.OnDeleteClicked += (x) =>
            {
                Remove(x.ItemObject);
            };

            treeItem.Header = gameObject;

            parent.Items.Add(treeItem);

            OnChildGameObjectAdd?.Invoke(this, value);
        }

        public void Clear()
        {
            Tree.Items.Clear();

            OnClear?.Invoke(this);
        }

        public HierarchyElement()
        {
            InitializeComponent();

            ContextMenu.Opened += OnContextMenuOpened;
            ContextMenu.Closed += OnContextMenuClosed;

            CreateButton.Click += OnCreateButtonClicked;

            Tree.SelectedItemChanged += OnSelectedItemChanged;
        }

        private void OnSelectedItemChanged(object _, RoutedPropertyChangedEventArgs<object> __)
        {
            OnSelectedChanged?.Invoke(this, SelectedGameObject);
        }

        private void OnCreateButtonClicked(object _, RoutedEventArgs __)
        {
            OnCreateNewGameObjectClicked?.Invoke(this);
        }

        private void OnContextMenuClosed(object _, RoutedEventArgs __)
        {}

        private void OnContextMenuOpened(object _, RoutedEventArgs __)
        {}

        private void ContextMenuCreateNewGameObjectClicked(object sender, RoutedEventArgs e)
        {
            OnCreateNewGameObjectClicked?.Invoke(this);
        }
    }
}
