using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneSystemManager : MonoBehaviour
{
    // Scene Fader
    Fader _fader;

    // Scene Tracking
    Scene _currentLevel;
    int _numOfScenes; // Number of total scenes in the game
    int _mainMenuIndex;
    int _titleIndex;

    private void Awake()
    {
        // If using the Unity editor or development build, enable debug logs
        Debug.unityLogger.logEnabled = Debug.isDebugBuild;

        // Cache child fader
        _fader = GetComponentInChildren<Fader>();

        // Get total number of scenes in game and indexes for main menu and gameplay scenes
        _numOfScenes = SceneManager.sceneCountInBuildSettings;
        _titleIndex = 3;
        _mainMenuIndex = 1;

        // Create level events
        EventManager.EventInitialise(EventType.LEVEL_STARTED);
        EventManager.EventInitialise(EventType.LEVEL_ENDED);
        EventManager.EventInitialise(EventType.FADING);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.LEVEL_SELECTED, LevelSelected);
        EventManager.EventSubscribe(EventType.QUIT_LEVEL, QuitLevelHandler);
        EventManager.EventSubscribe(EventType.QUIT_GAME, QuitGameHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.LEVEL_SELECTED, LevelSelected);
        EventManager.EventUnsubscribe(EventType.QUIT_LEVEL, QuitLevelHandler);
        EventManager.EventUnsubscribe(EventType.QUIT_GAME, QuitGameHandler);
    }

    // After Services Scene is loaded in, additively load in the MainMenu scene
    private void Start()
    {
        #if UNITY_EDITOR
            int count = SceneManager.loadedSceneCount;

            StartCoroutine(_fader.NormalFadeIn());

        #else
            StartCoroutine(Title());
        #endif

        
    }

    IEnumerator Title()
    {
        yield return StartCoroutine(LoadScene(_titleIndex));
        yield return StartCoroutine(_fader.NormalFadeIn());
        yield return StartCoroutine(_fader.NormalFadeOut());
        yield return StartCoroutine(UnloadScene(_titleIndex));
        yield return StartCoroutine(LoadScene(_mainMenuIndex));
        yield return StartCoroutine(_fader.NormalFadeIn());
    }

    #region Game UI Response
    // Listens for when UIManager QuitButton is pressed
    public void QuitLevelHandler(object data)
    {
        StartCoroutine(LevelToMenu());
    }
    #endregion

    #region Main Menu UI Response
    public void LevelSelected(object data)
    {
        if (data == null)
        {
            Debug.LogError("Level has not been chosen!");
        }

        int sceneIndex = (int)data + 2;
        StartCoroutine(MenuToLevel(sceneIndex));
    }
    #endregion

    #region Scene Loading/Unloading/Ordering
    IEnumerator LevelToMenu()
    {
        EventManager.EventTrigger(EventType.FADING, false);
        yield return StartCoroutine(_fader.NormalFadeOut());
        yield return StartCoroutine(UnloadLevel(_currentLevel.buildIndex));
        //yield return StartCoroutine(UnloadScene(_gameplayIndex));
        yield return StartCoroutine(LoadScene(_mainMenuIndex));
        yield return StartCoroutine(_fader.NormalFadeIn());
        EventManager.EventTrigger(EventType.FADING, true);
    }

    IEnumerator MenuToLevel(int levelSelected)
    {
        EventManager.EventTrigger(EventType.FADING, false);
        yield return StartCoroutine(_fader.NormalFadeOut());
        yield return StartCoroutine(UnloadScene(_mainMenuIndex));
        //yield return StartCoroutine(LoadScene(_gameplayIndex));
        yield return StartCoroutine(LoadLevel(levelSelected));
        yield return StartCoroutine(_fader.NormalFadeIn());
        EventManager.EventTrigger(EventType.FADING, true);
    }
    #endregion

    #region Level-Related Functions
    // Only loads levels, does not load MainMenu scene or core scenes
    IEnumerator LoadLevel(int index)
    {
        yield return StartCoroutine(LoadScene(index));
        EventManager.EventTrigger(EventType.LEVEL_STARTED, null);
        _currentLevel = SceneManager.GetSceneByBuildIndex(index);
    }

    // Only unloads levels
    IEnumerator UnloadLevel(int index)
    {
        EventManager.EventTrigger(EventType.LEVEL_ENDED, null);
        yield return StartCoroutine(UnloadScene(index));
    }
    #endregion

    #region Scene Functions
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

    IEnumerator UnloadScene(int index)
    {
        var levelAsync = SceneManager.UnloadSceneAsync(index);

        // Wait until the scene fully unloads
        while (!levelAsync.isDone)
        {
            yield return null;
        }
    }

    public void QuitGameHandler(object data)
    {
        StartCoroutine(QuitGame());
    }

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
