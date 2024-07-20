using Accessibility;
using Microsoft.Build.Framework;
using NetDock.Controls;
using SharpEngineCore.ECS;
using SharpEngineEditor.Misc;
using SharpEngineEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
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

    private GameAssembly _gameAssembly;
    private Assembly _engineCoreAssembly;

    private TypeResolver _componentTypeResolver;

    private void CreateBindings()
    {
        _hierarchy.OnCreateNewGameObjectClicked += OnCreateNewGameObjectClicked;
        _hierarchy.OnRootGameObjectAdd          += OnRootGameObjectAdd;
        _hierarchy.OnGameObjectRemove           += OnGameObjectRemove;
        _hierarchy.OnClear                      += OnSceneClear;
        _hierarchy.OnChildGameObjectAdd         += OnChildGameObjectAdd;
        _hierarchy.OnSelectedChanged            += OnGameObjectSelected;

        _inspector.OnAddComponentClicked += OnAddComponentClicked;

        _console.Log("Engine Bindings Created.");
    }

    private void OnAddComponentClicked(SharpEngineEditorControls.Components.InspectorElement inspector, string componentName)
    {
        if (componentName == null || componentName == string.Empty)
            return;

        var type = _componentTypeResolver.Resolve(componentName);
        if(type == null)
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
        method.Invoke(gameObject, null);

        _inspector.Refresh();
    }

    private void RemoveBindings()
    {
        _hierarchy.OnCreateNewGameObjectClicked     -= OnCreateNewGameObjectClicked;
        _hierarchy.OnRootGameObjectAdd              -= OnRootGameObjectAdd;
        _hierarchy.OnGameObjectRemove               -= OnGameObjectRemove;
        _hierarchy.OnClear                          -= OnSceneClear;
        _hierarchy.OnChildGameObjectAdd             -= OnChildGameObjectAdd;
        _hierarchy.OnSelectedChanged                -= OnGameObjectSelected;

        _inspector.OnAddComponentClicked -= OnAddComponentClicked;

        _console.Log("Engine Bindings Removed.");
    }

    #region BINDINGS
#nullable enable
    private void OnGameObjectSelected(SharpEngineEditorControls.Components.HierarchyElement hierarchy, object? @object)
    {
        if (@object == null)
        {
            _inspector.Clear();
            return;
        }

        var gameObject = (GameObject)@object;

        _inspector.Clear();
        var components = gameObject.GetAllComponents();

        foreach (var component in components)
        {
            _inspector.AddObject(component);
        }

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
        var gameObject = (GameObject)@object;
        SharpEngineCore.ECS.SceneManager.ActiveScene.ECS.Remove(gameObject);

        _inspector.Clear();
    }

    private void OnRootGameObjectAdd(SharpEngineEditorControls.Components.HierarchyElement hierarchy, object @object)
    {
        var gameObject = (GameObject)@object;
    }

    private void OnCreateNewGameObjectClicked(SharpEngineEditorControls.Components.HierarchyElement hierarchy)
    {
        var name = string.Empty;

        var gameObject = SharpEngineCore.ECS.SceneManager.ActiveScene.ECS.Create();
        gameObject.name = "Game Object";

        name = gameObject.name.Clone();

        hierarchy.AddRootGameObject(name, gameObject);
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

        var consoleDockItem = new DockItem(_console);
        {
            var name = _console.Name;
            consoleDockItem.Name = name;
            consoleDockItem.TabName = name;
        }

        var sharpEngineViewDockItem = new DockItem(_engineView);
        {
            var name = _engineView.Name;
            sharpEngineViewDockItem.Name = name;
            sharpEngineViewDockItem.TabName = name;
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

        DockSurface.Add(projectDockItem, NetDock.Enums.DockDirection.Bottom);
        DockSurface.Add(consoleDockItem, NetDock.Enums.DockDirection.Bottom);
        DockSurface.Add(sharpEngineViewDockItem, NetDock.Enums.DockDirection.Top);
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
    }

    private void OnEngineUnloaded(SharpEngineView obj)
    {
        _engineView.OnEngineUnloaded -= OnEngineUnloaded;

        _console.Log("Engine Unloaded");
        RemoveBindings();
    }

    private void OnEngineLoaded(SharpEngineView obj)
    {
        _engineView.OnEngineLoaded -= OnEngineLoaded;
        _console.Log("Engine Loaded");

        _engineCoreAssembly = _engineView.EngineCoreAssembly;
        _gameAssembly = _engineView.GameAssembly;

        _componentTypeResolver = new TypeResolver([_gameAssembly.Assembly, _engineCoreAssembly],
            typeof(SharpEngineCore.ECS.Component));

        var currentSceneName = string.Empty;
        _engineView.ENGINE_CALL(() =>
        {
            currentSceneName = SceneManager.ActiveScene.name;
        });

        _console.Log($"Loaded: {currentSceneName}");

        CreateBindings();
    }
}
