using SharpEngineCore.ECS.Components;
using System.Diagnostics;

namespace SharpEngineCore.ECS;

public sealed class GameObject
{
    public readonly Guid id = Guid.NewGuid();

    private const string DEFAULT_NAME = "GameObject";
    public string name = DEFAULT_NAME;

    internal List<Component> _pendingAdds = new();
    internal List<Component> _pendingRemove = new();
    internal List<Component> _components = new();

    public bool IsActive { get; private set; }

    public void SetActive(bool active)
    {
        IsActive = active;
    }

    internal void Tick(TickType tick)
    {
        ClearPendings();

        switch (tick)
        {
            case TickType.Start:
                Start();
                break;
            case TickType.Update:
                Update();
                break;
            case TickType.OnSpawn:
                OnSpawn();
                break;
            case TickType.OnDespawn:
                OnDespawn();
                break;
            case TickType.OnEnable:
                OnEnable();
                break;
            case TickType.OnDisable:
                OnDisable();
                break;
            default:
                Debug.Assert(false,
                    $"Unknown tick type for gameObject, {tick}");
                break;
        }

        void Start()
        {
            if (IsActive == false)
                return;

            foreach (var component in _components)
            {
                if (component.IsEnabled == false)
                    continue;

                component.Start();
            }
        }

        void Update()
        {
            if (IsActive == false)
                return;

            foreach (var component in _components)
            {
                if (component.IsEnabled == false)
                    continue;

                component.Update();
            }
        }

        void OnSpawn()
        {
            if (IsActive == false)
                return;

            foreach (var component in _components)
            {
                if (component.IsEnabled == false)
                    continue;

                component.OnSpawn();
            }
        }

        void OnDespawn()
        {
            if (IsActive == false)
                return;

            foreach (var component in _components)
            {
                if (component.IsEnabled == false)
                    continue;

                component.OnDestroy();
            }
        }

        void OnEnable()
        {
            Debug.Assert(IsActive == true);

            foreach (var component in _components)
            {
                if (component.IsEnabled == false)
                    continue;

                component.OnEnable();
            }
        }

        void OnDisable()
        {
            Debug.Assert(IsActive == false);

            foreach (var component in _components)
            {
                if (component.IsEnabled == false)
                    continue;

                component.OnDisable();
            }
        }

        void ClearPendings()
        {
            foreach(var component in _pendingAdds)
            {
                _components.Add(component);
                _pendingAdds.Remove(component);

                if (component.IsEnabled == false ||
                    IsActive == false)
                    continue;

                component.Awake();
            }

            foreach(var component in _pendingRemove)
            {
                _components.Remove(component);
                _pendingRemove.Remove(component);

                if (component.IsEnabled == false ||
                    IsActive            == false)
                    continue;

                component.OnDestroy();
            }
        }
    }

    public void RemoveComponent<T>()
        where T : Component, new()
    {
        var targets = GetComponents<T>();

        Debug.Assert(targets.Length > 0,
                $"No Component named {nameof(T)} Found on {name}");

        var target = targets.First();

        _pendingRemove.Add(target);
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
        _pendingAdds.Add(component);

        component.gameObject = this;

        return component;
    }

    internal GameObject()
    {
        AddComponent<Transform>();
    }
}
