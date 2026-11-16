using UnityEngine;
using UnityEngine.SceneManagement;

public static class SpatialRegistration
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= RegisterScene;
        SceneManager.sceneLoaded += RegisterScene;
    }
    private static void RegisterScene(Scene scene, LoadSceneMode mode)
    {
        // One-time registration for old scenes, never a perception search.
        foreach (var root in scene.GetRootGameObjects())
        foreach (var component in root.GetComponentsInChildren<MonoBehaviour>(true))
            if (component is Food || component is WaterSource || component is RestArea || component is Prey)
                if (component.GetComponent<SpatialMember>() == null)
                    component.gameObject.AddComponent<SpatialMember>();
    }
}
