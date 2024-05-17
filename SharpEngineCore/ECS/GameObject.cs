using System.Diagnostics;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.ECS;

public abstract class Component
{
    public readonly Guid Id = Guid.NewGuid();

    public readonly GameObject gameObject;

    protected Component(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }

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

}

public sealed class GameObject
{
    public readonly Guid id = Guid.NewGuid();

    private const string DEFAULT_NAME = "GameObject";
    public string name = DEFAULT_NAME;

    private List<Component> _components = new();

    public void RemoveComponent<T>()
        where T : Component, new()
    {
        var targets = GetComponents<T>();
        _components.Remove(targets.First());
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
        return component;
    }

    public GameObject()
    {
    }
}
