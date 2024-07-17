using HandyControl.Controls;
using SharpEngineEditorControls.Editors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Runtime.CompilerServices;

namespace SharpEngineEditorControls.Components
{
    /// <summary>
    /// Interaction logic for InspectorElement.xaml
    /// </summary>
    public partial class InspectorElement : System.Windows.Controls.UserControl
    {
        public event Action<InspectorElement, object> OnObjectAdd;
        public event Action<InspectorElement, object> OnObjectRemoved;

        public event Action<InspectorElement, SharpEngineEditorResolver> OnEditorResolverChanged;

        public event Action<InspectorElement> OnRefresh;
        public event Action<InspectorElement> OnClear;

        private List<object> _objects = new();

        public SharpEngineEditorResolver EditorResolver
        {
            get
            {
                return _resolver;
            }
            set
            {
                Debug.Assert(value != null);

                _resolver.Clean();

                _resolver = value;
                OnEditorResolverChanged?.Invoke(this, _resolver);
            }
        }
        private SharpEngineEditorResolver _resolver = new();

        public void AddObject(object @object)
        {
            _objects.Add(@object);
            Refresh();

            OnObjectAdd?.Invoke(this, @object);
        }

        public void RemoveObject(object @object)
        {
            _objects.Remove(@object);
            Refresh();

            OnObjectRemoved?.Invoke(this, @object);
        }

        public void Clear()
        {
            _objects.Clear();
            ComponentsStack.Children.Clear();
            _resolver.Clean();

            OnClear?.Invoke(this);
        }

        public void Refresh()
        {
            ComponentsStack.Children.Clear();
            _resolver.Clean();

            SerializeUI();

            OnRefresh?.Invoke(this);
        }

        private void SerializeUI()
        {
            foreach (var @object in _objects)
            {
                var type = @object.GetType();
                var editor = _resolver.Resolve(type);

                var uiCollection = editor.CreateUI(_resolver, new(@object, null));
                uiCollection.Margin = new System.Windows.Thickness(0, 10, 0, 0);

                ComponentsStack.Children.Add(uiCollection);
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        public InspectorElement()
        {
            InitializeComponent();
        }
    }
}