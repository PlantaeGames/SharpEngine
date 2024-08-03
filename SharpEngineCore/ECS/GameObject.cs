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

    public Transform Transform { get; private set; }
    public bool IsActive { get; private set; }

    public void SetActive(bool active)
    {
        IsActive = active;
    }

    internal void Tick(TickType tick)
    {
        if(SceneManager.IsPlaying)
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
            case TickType.OnPreRender:
                OnPreRender();
                break;
            case TickType.OnPostRender:
                OnPostRender();
                break;
            default:
                Debug.Assert(false,
                    $"Unknown tick type for gameObject, {tick}");
                break;
        }

        void OnPreRender()
        {
            if (IsActive == false)
                return;

            if (SceneManager.IsPlaying)
            {
                foreach (var component in _components)
                {
                    if (component.IsEnabled == false)
                        continue;

                    component.OnPreRender();
                }
            }
            else
            {
                foreach (var component in _pendingAdds)
                {
                    if (component.IsEnabled == false)
                        continue;

                    component.OnPreRender();
                }
            }
        }
        void OnPostRender()
        {
            if (IsActive == false)
                return;

            if (SceneManager.IsPlaying)
            {
                foreach (var component in _components)
                {
                    if (component.IsEnabled == false)
                        continue;

                    component.OnPostRender();
                }
            }
            else
            {
                foreach (var component in _pendingAdds)
                {
                    if (component.IsEnabled == false)
                        continue;

                    component.OnPostRender();
                }
            }
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
            foreach (var component in _pendingAdds)
            {
                _components.Add(component);
                component.Awake();
            }

            _pendingAdds.Clear();

            foreach (var component in _pendingRemove)
            {
                _components.Remove(component);
                _pendingRemove.Remove(component);

                component.OnDestroy();
            }

            _pendingRemove.Clear();
        }
    }

    public void RemoveComponent<T>()
        where T : Component, new()
    {
        var targets = GetComponents<T>();

        Debug.Assert(targets.Length > 0,
                $"No Component named {nameof(T)} Found on {name}");

        var target = targets.First();

        Debug.Assert(target as Transform == null,
            $"Can't remove Transform component, {name}");

        if (SceneManager.IsPlaying)
            _pendingRemove.Add(target);
        else
        {
            _pendingAdds.Remove(target);
        }
    }

    public T[] GetComponents<T>()
        where T : Component, new()
    {
        var components = new List<Component>();
        components.AddRange(_components);
        components.AddRange(_pendingAdds);

        var targets = components
                .Where(x => x as T != null);
        
        Debug.Assert(targets.Count() > 0,
            $"Component: {nameof(T)} does not found on gameObject: {name}");

        return targets.Cast<T>().ToArray();
    }

    public Component[] GetAllComponents()
    {
        var components = new List<Component>();
        components.AddRange(_components);
        components.AddRange(_pendingAdds);

        return [.. components];
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
        Debug.Assert(Transform == null? true : typeof(T) != typeof(Transform),
            "GameObjects can't have more than one Transform.");

        var component = new T();
        _pendingAdds.Add(component);

        component.gameObject = this;

        return component;
    }

    internal GameObject()
    {
        Transform = AddComponent<Transform>();
    }
}
