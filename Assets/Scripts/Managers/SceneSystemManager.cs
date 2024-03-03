using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneSystemManager : MonoBehaviour
{
    // Scene Fader
    [SerializeField] private Fader _fader;

    // Scene Tracking
    Scene _currentLevel;
    int _numOfScenes; // Number of total scenes in the game
    int _mainMenuIndex;

    private void Awake()
    {
        // If using the Unity editor or development build, enable debug logs
        Debug.unityLogger.logEnabled = Debug.isDebugBuild;

        // Get total number of scenes in game and indexes for main menu and gameplay scenes
        _numOfScenes = SceneManager.sceneCountInBuildSettings;
        _mainMenuIndex = 1;

        EventManager.EventInitialise(EventType.FADING);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.QUIT_GAME, QuitGameHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.QUIT_GAME, QuitGameHandler);
    }

    // After Services Scene is loaded in, additively load in the MainMenu scene
    private void Start()
    {
        // If unity is in the editor, load current scene, otherwise load main menu scene
        #if UNITY_EDITOR
            int count = SceneManager.loadedSceneCount;

            StartCoroutine(_fader.NormalFadeIn());

        #else
            StartCoroutine(LoadScene(_mainMenuIndex));
        #endif 
    }

    // IEnumerator MenuToLevel(int levelSelected)
    // {
    //     EventManager.EventTrigger(EventType.FADING, false);
    //     yield return StartCoroutine(_fader.NormalFadeOut());
    //     yield return StartCoroutine(UnloadScene(_mainMenuIndex));
    //     yield return StartCoroutine(LoadLevel(levelSelected));
    //     yield return StartCoroutine(_fader.NormalFadeIn());
    //     EventManager.EventTrigger(EventType.FADING, true);
    // }

    #region SCENE FUNCTIONS
    // Loads specified scene
    IEnumerator LoadScene(int index)
    {
        var levelAsync = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);

        // Wait until the scene fully loads to fade in
        while (!levelAsync.isDone)
        {
            yield return null;
        }

        Scene scene = SceneManager.GetSceneAt(SceneManager.loadedSceneCount - 1);
        SceneManager.SetActiveScene(scene);
        _currentLevel = scene;
    }

    // Unloads specified scene
    IEnumerator UnloadScene(int index)
    {
        var levelAsync = SceneManager.UnloadSceneAsync(index);

        // Wait until the scene fully unloads
        while (!levelAsync.isDone)
        {
            yield return null;
        }
    }

    // Starts QuitGame Coroutine
    public void QuitGameHandler(object data)
    {
        StartCoroutine(QuitGame());
    }

    // Quits game in either build or editor
    IEnumerator QuitGame()
    {
        yield return StartCoroutine(_fader.NormalFadeOut());
        yield return null;

        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
	    	Application.Quit();
        #endif
    }
    #endregion
}
