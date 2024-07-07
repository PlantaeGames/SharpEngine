using System.Diagnostics;

namespace SharpEngineCore.ECS;

public static class SceneManager
{
    public static Scene ActiveScene { get; private set; } = new Scene();

    private static List<Scene> _scenes = new();

    internal static void Tick(TickType tick)
    {
        ActiveScene.Tick(tick);
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

    static SceneManager()
    {
        AddScene(ActiveScene);
    }
}
