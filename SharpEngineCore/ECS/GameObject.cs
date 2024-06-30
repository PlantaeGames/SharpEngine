using System.Diagnostics;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.ECS;

internal enum TickType
{
    Start,
    Update,
}

public sealed class ECS
{
    public int ActiveSceneIndex;
    public List<Scene> Scenes = new();

    public Scene GetActiveScene
    {
        get
        {
            Debug.Assert(Scenes.Count > ActiveSceneIndex,
                         $"Active Scene Index Out of Range.");

            return Scenes[ActiveSceneIndex];
        }
    }

    internal void Tick(TickType type)
    {
        Debug.Assert(Scenes.Count > ActiveSceneIndex,
                     $"Active Scene Index Out of Range.");

        var scene = Scenes[ActiveSceneIndex];

        switch (type)
        {
            case TickType.Start:
                StartTick();
                break;
            case TickType.Update:
                UpdateTick();
                break;
            default:
                Debug.Assert(false,
                            $"{nameof(ECS)}: Unknown {nameof(TickType)}, {type}");
                break;
        }

        void StartTick()
        {
            foreach(var gameObj in scene.GameObjects)
            {
                if (gameObj.IsActive == false)
                    continue;

                foreach (var component in gameObj._components)
                {
                    if (component.IsEnabled == false)
                        continue;

                    component.Start();
                }
            }
        }

        void UpdateTick()
        {
            foreach (var gameObj in scene.GameObjects)
            {
                if (gameObj.IsActive == false)
                    continue;

                foreach (var component in gameObj._components)
                {
                    if (component.IsEnabled == false)
                        continue;

                    component.Update();
                }
            }
        }
    }
}

public abstract class Component
{
    public readonly Guid Id = Guid.NewGuid();
    public bool IsEnabled;

#nullable disable
    public GameObject gameObject { get; internal set; }
#nullable enable


    protected Component()
    {}

    public virtual void Awake()
    { }
    public virtual void Start()
    { }
    public virtual void Update()
    { }
    public virtual void FixedUpdate()
    { }
    public virtual void LateUpdate()
    { }
    public virtual void OnDestroy()
    { }
    public virtual void OnDisable()
    { }
    public virtual void OnEnable()
    { }
}

public sealed class Scene
{
    private const string DEFAULT_NAME = "Scene";

    public readonly Guid Id = Guid.NewGuid();

    public string name = DEFAULT_NAME;
    internal List<GameObject> GameObjects = new();
    internal List<GameObject> PendingAddGameObjects = new();
    internal List<GameObject> PendingRemoveGameObjects = new();
}

public sealed class GameObject
{
    public readonly Guid id = Guid.NewGuid();

    private const string DEFAULT_NAME = "GameObject";
    public string name = DEFAULT_NAME;

    internal List<Component> _components = new();

    public bool IsActive { get; private set; }

    public void SetActive(bool active)
    {
        IsActive = active;
    }

    public void RemoveComponent<T>()
        where T : Component, new()
    {
        var targets = GetComponents<T>();
        var target = targets.First();

        target.OnDestroy();

        _components.Remove(target);
    }

    public T[] GetComponents<T>()
        where T : Component, new()
    {
        var targets = _components
                .Where(x => x as T != null);
        Debug.Assert(targets.Count() > 0,
            $"Component: {nameof(T)} does not found on gameObject: {name}");

        var components = new List<T>();
        foreach (var component in targets)
        {
            components.Add((T)component);
        }

        return components.ToArray();
    }

    public T GetComponent<T>()
        where T : Component, new()
    {
        var targets = GetComponents<T>();
        return targets.First();
    }

    public T AddComponent<T>()
        where T : Component, new()
    {
        var component = new T();
        _components.Add(component);

        component.gameObject = this;

        component.Awake();

        

        return component;
    }

    public GameObject()
    {

    }
}
