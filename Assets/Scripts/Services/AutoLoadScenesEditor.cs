#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class AutoLoadScenesEditor
{
    public const string ServicesSceneName = "Services";
    public const string ServicesPath = "Assets/Scenes/Core/Services.unity";

    public const string GameplaySceneName = "Gameplay";
    public const string GameplayPath = "Assets/Scenes/Core/Gameplay.unity";

    static AutoLoadScenesEditor()
    {
        EditorSceneManager.sceneOpened += LoadServicesScene;
        EditorSceneManager.sceneOpened += LoadGameplayScene;
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

    // Automatically load the Gameplay scene if Levels has been loaded as a single
    static void LoadGameplayScene(Scene scene, OpenSceneMode mode)
    {
        // Make sure additive scene loading hasn't happened or Services/Gameplay/Main Menu hasn't just been loaded in
        if (mode == OpenSceneMode.Single && scene.name != GameplaySceneName && scene.name != "MainMenu" && scene.name != ServicesSceneName)
        {
            Scene gameplay = EditorSceneManager.GetSceneByPath(GameplayPath);

            // Check if Gameplay hasn't already been loaded in before
            if (!gameplay.isLoaded)
            {
                gameplay = EditorSceneManager.OpenScene(GameplayPath, OpenSceneMode.Additive);
            }

            // Move to top of hierarchy
            EditorSceneManager.MoveSceneBefore(gameplay, scene);
        }
    }
}
#endif