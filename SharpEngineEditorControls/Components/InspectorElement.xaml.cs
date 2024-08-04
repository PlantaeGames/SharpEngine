using SharpEngineEditorControls.Editors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using SharpEngineEditorControls.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Linq.Expressions;
using System.Threading.Tasks;

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
        public event Action<InspectorElement, string, object> OnRemoveClicked;

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

        private Thread _refreshThread;
        private bool _refreshThreadQuitRequested;
        private object _refreshExitLock = new();

        private const int REFRESH_RATE = 1;

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
                componentElement.AddCollection(type.Name, uiCollection, @object);
                componentElement.ToggleExpend(true);
                componentElement.Margin = new System.Windows.Thickness(0, 10, 0, 0);

                componentElement.OnRemove += x =>
                {
                    OnRemoveClicked?.Invoke(this, x.Name, @object);
                };

                ComponentsStack.Children.Add(componentElement);
            }

            ComponentsStack.Children.Add(_addComponentElement);
        }

        public InspectorElement(object @lock)
        {
            _lock = @lock;

            this.Loaded += OnLoaded;
            this.Unloaded += OnUnLoaded;

            InitializeComponent();
        }

        private void OnUnLoaded(object _, RoutedEventArgs __)
        {
            this.Unloaded -= OnUnLoaded;

            lock (_refreshExitLock)
            {
                _refreshThreadQuitRequested = true;
            }

            _refreshThread.Join();
        }

        private void OnLoaded(object _, System.Windows.RoutedEventArgs __)
        {
            this.Loaded -= OnLoaded;

            _addComponentElement = new AddComponentElement();
            _addComponentElement.Margin = new System.Windows.Thickness(10);
            _addComponentElement.OnAddClicked += (_, name) =>
            {
                OnAddClicked?.Invoke(this, name);
            };

            _refreshThread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    lock(_refreshExitLock)
                    {
                        if (_refreshThreadQuitRequested)
                            break;
                    }

                    if (Dispatcher.HasShutdownStarted)
                        break;

                    try
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            lock (_lock)
                                _resolver.Refresh();
                        }, DispatcherPriority.ApplicationIdle);
                    }
                    catch(TaskCanceledException _)
                    {}

                    Thread.Sleep((int)((1f / REFRESH_RATE) * 1000));
                }
            }));
            _refreshThread.Start();
        }
    }
}