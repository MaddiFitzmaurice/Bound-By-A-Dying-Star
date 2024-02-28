#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class AutoLoadScenesEditor
{
    public const string ServicesSceneName = "Services";
    public const string ServicesPath = "Assets/Scenes/Services.unity";

    static AutoLoadScenesEditor()
    {
        EditorSceneManager.sceneOpened += LoadServicesScene;
    }

    // Automatically load the Services scene if Main Menu or Levels has been loaded as a single
    static void LoadServicesScene(Scene scene, OpenSceneMode mode)
    {
        // Make sure additive scene loading hasn't happened or Services hasn't been loaded in
        if (mode == OpenSceneMode.Single && scene.name != ServicesSceneName)
        {
            Scene services = EditorSceneManager.GetSceneByPath(ServicesPath);

            // Check if Services hasn't already been loaded in before
            if (!services.isLoaded)
            {
                services = EditorSceneManager.OpenScene(ServicesPath, OpenSceneMode.Additive);
            }

            // Move to top of hierarchy
            EditorSceneManager.MoveSceneBefore(services, scene);
        }
    }
}
#endif