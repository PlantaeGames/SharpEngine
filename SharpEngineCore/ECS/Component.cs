namespace SharpEngineCore.ECS;

public abstract class Component
{
    public readonly Guid Id = Guid.NewGuid();
    public bool IsEnabled = true;

#nullable disable
    public GameObject gameObject { get; internal set; }
#nullable enable

    protected Component()
    {}

    public virtual void OnSpawn()
    { }
    
    public virtual void Awake()
    { }
    public virtual void Start()
    { }
    public virtual void Update()
    { }

    public virtual void OnDestroy()
    { }

    public virtual void OnEnable()
    { }

    public virtual void OnDisable() 
    { }
}
