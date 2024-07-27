using System.Diagnostics;

namespace SharpEngineCore.ECS;

public sealed class ECS
{
    private List<GameObject> _gameObjects = new();
    private List<GameObject> _pendingAddGameObjects = new();
    private List<GameObject> _pendingRemoveGameObjects = new();
    private List<GameObject> _disabledGameObjects = new();

    public GameObject Create()
    {
        var gameObject = new GameObject();

        _pendingAddGameObjects.Add(gameObject);

        return gameObject;
    }

    public void Remove(GameObject gameObject)
    {
        if(SceneManager.IsPlaying)
            _pendingRemoveGameObjects.Add(gameObject);
        else
            _gameObjects.Remove(gameObject);
    }

    internal void Tick(TickType tick)
    {
        ClearPendings();

        switch (tick)
        {
            case TickType.Start:
                StartTick();
                break;
            case TickType.Update:
                UpdateTick();
                break;
            default:
                Debug.Assert(false,
                            $"{nameof(ECS)}: Unknown {nameof(TickType)}, {tick}");
                break;
        }

        void StartTick()
        {
            foreach(var gameObj in _gameObjects)
            {
                gameObj.Tick(tick);
            }
        }

        void UpdateTick()
        {
            foreach (var gameObj in _gameObjects)
            {
                gameObj.Tick(tick);
            }
        }

        void ClearPendings()
        {
            var newDisabled = new List<GameObject>();
            var newActived = new List<GameObject>();

            // on disable
            foreach(var gameObject in _gameObjects)
            {
                if (gameObject.IsActive)
                    continue;

                gameObject.Tick(TickType.OnDisable);

                newDisabled.Add(gameObject);
            }

            foreach(var gameObject in newDisabled)
            {
                _gameObjects.Remove(gameObject);
            }

            foreach(var gameObject in _disabledGameObjects)
            {
                if (gameObject.IsActive == false)
                    continue;

                newActived.Add(gameObject);
            }

            foreach(var gameObject in newActived)
            {
                _disabledGameObjects.Remove(gameObject);
            }

            _gameObjects.AddRange(newActived);

            // on enable
            foreach(var gameObject in newActived)
            {
                gameObject.Tick(TickType.OnEnable);
            }

            _disabledGameObjects.AddRange(newDisabled);


            // on spawn
            foreach (var gameObject in _pendingAddGameObjects)
            {
                _gameObjects.Add(gameObject);

                gameObject.Tick(TickType.OnSpawn);
            }

            // on despawn
            foreach (var gameObject in _pendingRemoveGameObjects)
            {
                gameObject.Tick(TickType.OnDespawn);

                _gameObjects.Remove(gameObject);
            }

            _pendingRemoveGameObjects.Clear();
            _pendingAddGameObjects.Clear();
        }
    }
}
