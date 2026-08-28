using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class PlayerPrefsService : IPersistenceService
{
    public int GetInt(string key, int defaultValue) => PlayerPrefs.GetInt(key, defaultValue);
    public void SetInt(string key, int value) => PlayerPrefs.SetInt(key, value);
    public void Save() => PlayerPrefs.Save();
}

public sealed class SceneService : ISceneService
{
    public void Load(string sceneName) => SceneManager.LoadScene(sceneName);
}

public static class CoreServiceRegistration
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register()
    {
        ServiceLocator.Reset();
        ServiceLocator.Register<IPersistenceService>(new PlayerPrefsService());
        ServiceLocator.Register<ISceneService>(new SceneService());
        ServiceLocator.Register<IUIFactory>(new UIFactoryService());
    }
}
