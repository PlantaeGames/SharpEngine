using System.Diagnostics;

namespace SharpEngineCore.ECS;

public static class SceneManager
{
    public static Scene ActiveScene { get; private set; }

    private static List<Scene> _scenes = new();
    internal static bool IsPlaying { get; private set; }

    internal static void Start()
    {
        IsPlaying = true;
    }

    internal static void Stop()
    {
        IsPlaying = false;
    }

    internal static void Tick(TickType tick)
    {
        if (IsPlaying)
        {
            ActiveScene.Tick(tick);
        }
    }

    public static void LoadScene(string name)
    {
        foreach(var scene in _scenes)
        {
            if (scene.name != name)
                continue;

            ActiveScene = scene;
            return;
        }

        Debug.Assert(false,
            $"Scene named {name} has not added in {nameof(SceneManager)}");
    }

    public static void AddScene(Scene scene)
    {
        _scenes.Add(scene);
    }

    public static void RemoveScene(Scene scene)
    {
        _scenes.Remove(scene);
    }

    internal static void Initialize()
    {
        const string sceneName = "SampleScene";

        var scene = new Scene();
        scene.name = sceneName;

        AddScene(scene);
        LoadScene(sceneName);
    }
}
