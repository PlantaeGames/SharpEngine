﻿using SharpEngineEditorControls.Editors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using SharpEngineEditorControls.Controls;
using System.Windows.Media;

namespace SharpEngineEditorControls.Components
{
    /// <summary>
    /// Interaction logic for InspectorElement.xaml
    /// </summary>
    public partial class InspectorElement : System.Windows.Controls.UserControl
    {
        public event Action<InspectorElement, object> OnObjectAdd;
        public event Action<InspectorElement, object> OnObjectRemoved;
        public event Action<InspectorElement, string> OnAddClicked;
        public event Action<InspectorElement, string> OnRemoveClicked;

        public event Action<InspectorElement, SharpEngineEditorResolver> OnEditorResolverChanged;

        public event Action<InspectorElement> OnRefresh;
        public event Action<InspectorElement> OnClear;

        private List<object> _objects = new();

        private readonly object _lock;

        private AddComponentElement _addComponentElement;

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
            OnObjectAdd?.Invoke(this, @object);
        }

        public void RemoveObject(object @object)
        {
            _objects.Remove(@object);
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

            OnRefresh?.Invoke(this);

            SerializeUI();
        }

        private void SerializeUI()
        {
            foreach (var @object in _objects)
            {
                var type = @object.GetType();
                var editor = _resolver.Resolve(type);

                var uiCollection = editor.CreateUI(
                    _resolver, new(@object, null, _lock));

                var componentElement = new ComponentElement();
                componentElement.AddCollection(type.Name, uiCollection);
                componentElement.ToggleExpend(true);
                componentElement.Margin = new System.Windows.Thickness(0, 10, 0, 0);

                componentElement.OnRemove += x =>
                {
                    OnRemoveClicked?.Invoke(this, x.Name);
                };

                ComponentsStack.Children.Add(componentElement);
            }

            ComponentsStack.Children.Add(_addComponentElement);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _addComponentElement = new AddComponentElement();
            _addComponentElement.Margin = new System.Windows.Thickness(10);
            _addComponentElement.OnAddClicked += (_, name) =>
            {
                OnAddClicked?.Invoke(this, name);
            };
        }

        public InspectorElement(object @lock)
        {
            _lock = @lock;

            InitializeComponent();

            CompositionTarget.Rendering += OnUIRender;
        }

        private void OnUIRender(object _, EventArgs __)
        {
            _resolver.Refresh();
        }
    }
}