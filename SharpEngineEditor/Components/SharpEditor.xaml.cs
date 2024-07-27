using Accessibility;
using Microsoft.Build.Framework;
using NetDock.Controls;
using SharpEngineCore.ECS;
using SharpEngineCore.Graphics;
using SharpEngineEditor.Misc;
using SharpEngineEditor.Utilities;
using System;
using System.Collections.Generic;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TerraFX.Interop.WinRT;

namespace SharpEngineEditor.Components;

/// <summary>
/// Interaction logic for SharpEditor.xaml
/// </summary>
public partial class SharpEditor : UserControl
{
    private SharpEngineEditorControls.Components.HierarchyElement _hierarchy;
    private SharpEngineEditorControls.Components.ConsoleElement _console;
    private SharpEngineEditorControls.Components.InspectorElement _inspector;
    private SharpEngineEditorControls.Components.ProjectElement _project;
    private SharpEngineView _engineView;
    private SharpEngineSecondaryView _engineSecondaryView;

    private GameAssembly _gameAssembly;
    private Assembly _engineCoreAssembly;

    private TypeResolver _componentTypeResolver;

    private CameraObject _sceneCamera;

    private void CreateBindings()
    {
        _hierarchy.OnCreateNewGameObjectClicked += OnCreateNewGameObjectClicked;
        _hierarchy.OnGameObjectRemove           += OnGameObjectRemove;
        _hierarchy.OnClear                      += OnSceneClear;
        _hierarchy.OnChildGameObjectAdd         += OnChildGameObjectAdd;
        _hierarchy.OnSelectedChanged            += OnGameObjectSelected;

        _inspector.OnAddClicked                 += OnAddComponentClicked;
        _inspector.OnRefresh                    += OnInspectorRefresh;
        _inspector.OnRemoveClicked              += OnRemoveComponentClicked;

        _console.Log("Engine Bindings Created.");
    }

    private void RemoveBindings()
    {
        _hierarchy.OnCreateNewGameObjectClicked     -= OnCreateNewGameObjectClicked;
        _hierarchy.OnGameObjectRemove               -= OnGameObjectRemove;
        _hierarchy.OnClear                          -= OnSceneClear;
        _hierarchy.OnChildGameObjectAdd             -= OnChildGameObjectAdd;
        _hierarchy.OnSelectedChanged                -= OnGameObjectSelected;

        _inspector.OnAddClicked                     -= OnAddComponentClicked;
        _inspector.OnRefresh                        -= OnInspectorRefresh;
        _inspector.OnRemoveClicked                  -= OnRemoveComponentClicked;

        _console.Log("Engine Bindings Removed.");
    }

    #region BINDINGS

    private void OnRemoveComponentClicked(SharpEngineEditorControls.Components.InspectorElement inspector, string name)
    {
        var gameObject = (GameObject)_hierarchy.SelectedGameObject;

        var type = _componentTypeResolver.Resolve(name);
        var method = typeof(GameObject).GetMethod(nameof(GameObject.RemoveComponent));
        method = method.MakeGenericMethod(type);

        _engineView.ENGINE_CALL(() =>
        {
            method.Invoke(gameObject, null);
        });

        inspector.RemoveObject(gameObject);
        inspector.Refresh();
    }

    private void OnInspectorRefresh(SharpEngineEditorControls.Components.InspectorElement inspector)
    {
        var gameObject = (GameObject)_hierarchy.SelectedGameObject;
        if (gameObject == null)
        {
            _inspector.Clear();
            return;
        }

        _inspector.Clear();

        _engineView.ENGINE_CALL(() =>
        {
            var components = gameObject.GetAllComponents();

            foreach (var component in components)
            {
                _inspector.AddObject(component);
            }
        });
    }

    private void OnAddComponentClicked(SharpEngineEditorControls.Components.InspectorElement inspector, string componentName)
    {
        if (componentName == null ||
            componentName == string.Empty)
            return;

        var type = _componentTypeResolver.Resolve(componentName);
        if (type == null)
        {
            _console.Log($"Failed to add component, {componentName}" +
                         $"\nComponent not found.",
                         SharpEngineEditorControls.Components.ConsoleElement.LogType.Error);

            return;
        }

        var method = typeof(SharpEngineCore.ECS.GameObject).GetMethod(
            nameof(SharpEngineCore.ECS.GameObject.AddComponent));

        var gameObject = _hierarchy.SelectedGameObject;

        method = method.MakeGenericMethod(type);
        _engineView.ENGINE_CALL(() =>
        {
            method.Invoke(gameObject, null);
        });

        _inspector.Refresh();
    }

#nullable enable
    private void OnGameObjectSelected(SharpEngineEditorControls.Components.HierarchyElement hierarchy, object? @object)
    {
        _inspector.Refresh();
    }
#nullable disable

    private void OnChildGameObjectAdd(SharpEngineEditorControls.Components.HierarchyElement arg1, object arg2)
    {
        throw new NotImplementedException();
    }

    private void OnSceneClear(SharpEngineEditorControls.Components.HierarchyElement obj)
    {
        throw new NotImplementedException();
    }

    private void OnGameObjectRemove(SharpEngineEditorControls.Components.HierarchyElement hierarchy, object @object)
    {
        _engineView.ENGINE_CALL(() =>
        {
            var gameObject = (GameObject)@object;
            SharpEngineCore.ECS.SceneManager.ActiveScene.ECS.Remove(gameObject);
        });

        _inspector.Clear();
    }

    private void OnCreateNewGameObjectClicked(SharpEngineEditorControls.Components.HierarchyElement hierarchy)
    {
        _engineView.ENGINE_CALL(() =>
        {
            var gameObject = SharpEngineCore.ECS.SceneManager.ActiveScene.ECS.Create();
            gameObject.name = "Game Object";

            var name = new Span<Char>(gameObject.name.ToArray());

            hierarchy.AddRootGameObject(name.ToString(), gameObject);
        });
    }
    #endregion

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        _hierarchy = new SharpEngineEditorControls.Components.HierarchyElement();
        _console = new SharpEngineEditorControls.Components.ConsoleElement();
        _inspector = new SharpEngineEditorControls.Components.InspectorElement();
        _project = new SharpEngineEditorControls.Components.ProjectElement();
        _engineView = new SharpEngineView();
        _engineSecondaryView = new SharpEngineSecondaryView();

        var consoleDockItem = new DockItem(_console);
        {
            var name = _console.Name;
            consoleDockItem.Name = name;
            consoleDockItem.TabName = name;
        }

        var hierarchyDockItem = new DockItem(_hierarchy);
        {
            var name = _hierarchy.Name;
            hierarchyDockItem.Name = name;
            hierarchyDockItem.TabName = name;
        }

        var inspectorDockItem = new DockItem(_inspector);
        {
            var name = _inspector.Name;
            inspectorDockItem.Name = name;
            inspectorDockItem.TabName = name;
        }

        var projectDockItem = new DockItem(_project);
        {
            var name = _project.Name;
            projectDockItem.Name = name;
            projectDockItem.TabName = name;
        }
        var sharpEngineViewDockItem = new DockItem(_engineView);
        {
            var name = _engineView.Name;
            sharpEngineViewDockItem.Name = name;
            sharpEngineViewDockItem.TabName = name;
        }
        var sharpEngineSecodnaryViewDockItem = new DockItem(_engineSecondaryView);
        {
            var name = _engineSecondaryView.Name;
            sharpEngineSecodnaryViewDockItem.Name = name;
            sharpEngineSecodnaryViewDockItem.TabName = name;
        }

        DockSurface.Add(sharpEngineSecodnaryViewDockItem, NetDock.Enums.DockDirection.Top);
        DockSurface.Add(sharpEngineViewDockItem, NetDock.Enums.DockDirection.Top);
        DockSurface.Add(projectDockItem, NetDock.Enums.DockDirection.Bottom);
        DockSurface.Add(consoleDockItem, NetDock.Enums.DockDirection.Bottom);
        DockSurface.Add(hierarchyDockItem, NetDock.Enums.DockDirection.Left);
        DockSurface.Add(inspectorDockItem, NetDock.Enums.DockDirection.Right);

        _inspector.Clear();
        _hierarchy.Clear();
        _console.Clear();
    }
    public SharpEditor()
    {
        InitializeComponent();

        _engineView.OnEngineLoaded += OnEngineLoaded;
        _engineView.OnEngineUnloaded += OnEngineUnloaded;

        _engineSecondaryView.OnWindowCreated += OnEngineSecondaryWindowCreated;
        _engineSecondaryView.OnWindowDestroyed += OnEngineSecondaryWindowDestroyed;
    }

    private void OnEngineSecondaryWindowDestroyed(SharpEngineSecondaryView _)
    {
        _engineView.RemoveSecondaryView();
    }

    private void OnEngineSecondaryWindowCreated(SharpEngineSecondaryView _)
    {
        var camera = _engineView.AssignSecondaryView(_engineSecondaryView);
        _sceneCamera = camera;
    }

    private void OnEngineUnloaded(SharpEngineView _)
    {
        _engineView.OnEngineUnloaded -= OnEngineUnloaded;

        _console.Log("Engine Unloaded");
        RemoveBindings();
    }

    private void OnEngineLoaded(SharpEngineView _)
    {
        _engineView.OnEngineLoaded -= OnEngineLoaded;
        _console.Log("Engine Loaded");

        _engineSecondaryView.Initialize();

        _engineCoreAssembly = _engineView.EngineCoreAssembly;
        _gameAssembly = _engineView.GameAssembly;

        _componentTypeResolver = new TypeResolver([_gameAssembly.Assembly, _engineCoreAssembly],
            typeof(SharpEngineCore.ECS.Component));

        _engineView.ENGINE_CALL(() =>
        {
            var currentSceneName = new Span<char>(SceneManager.ActiveScene.name.ToCharArray());
            _console.Log($"Loaded: {currentSceneName}");
        });

        CreateBindings();
    }
}