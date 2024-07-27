using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using System.Collections.Generic;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneSystemManager : MonoBehaviour
{
    // Scene Fader
    #region EXTERNAL DATA
    [SerializeField] private Fader _fader;
    #endregion

    #region INTERNAL DATA
    // Scene Tracking
    private Scene _currentLevel;
    private int _numOfScenes; // Number of total scenes in the game
    private int _mainMenuIndex;
    private int _gameplayIndex;
    private int _servicesIndex;
    #endregion

    private void Awake()
    {
        // If using the Unity editor or development build, enable debug logs
        Debug.unityLogger.logEnabled = Debug.isDebugBuild;

        // Get total number of scenes in game and indexes for main menu, gameplay, and services scenes
        _numOfScenes = SceneManager.sceneCountInBuildSettings;
        _mainMenuIndex = GetBuildIndex("MainMenu");
        _gameplayIndex = GetBuildIndex("Gameplay");
        _servicesIndex = GetBuildIndex("Services");

        // Init Events
        EventManager.EventInitialise(EventType.FADING);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PLAY_GAME, PlayGameHandler);
        EventManager.EventSubscribe(EventType.QUIT_GAME, QuitGameHandler);
        EventManager.EventSubscribe(EventType.MAIN_MENU, MainMenuHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAY_GAME, PlayGameHandler);
        EventManager.EventUnsubscribe(EventType.QUIT_GAME, QuitGameHandler);
        EventManager.EventUnsubscribe(EventType.MAIN_MENU, MainMenuHandler);
    }

    // After Services Scene is loaded in, additively load in the MainMenu scene
    private void Start()
    {
        // If build is running, load in the main menu
        // Otherwise, unload all scenes except Services and reload in order of their layer architecture
#if !UNITY_EDITOR
        StartCoroutine(LoadScene(_mainMenuIndex));
        StartCoroutine(_fader.NormalFadeIn());
#else
        int loadedScenesCount = SceneManager.loadedSceneCount;
        Queue<int> loadedScenes = new Queue<int>();

        for (int i = 0; i < loadedScenesCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            if (scene.buildIndex != _servicesIndex)
            {
                loadedScenes.Enqueue(scene.buildIndex);
                SceneManager.UnloadSceneAsync(scene);
            }
        }

        StartCoroutine(ReloadAllScenes(loadedScenes));

#endif
    }

    #region LEVEL AND MENU FUNCTIONALITY
    // To simulate when the build boots up for the first time
    IEnumerator ReloadAllScenes(Queue<int> scenesToReload)
    {
        foreach (int i in scenesToReload)
        {
            yield return StartCoroutine(LoadScene(i));
        }
    }

    IEnumerator LevelChanger(int prevLevel, int newLevel)
    {
        EventManager.EventTrigger(EventType.FADING, false);
        yield return StartCoroutine(_fader.NormalFadeOut());
        yield return StartCoroutine(UnloadLevel(prevLevel));
        yield return StartCoroutine(LoadLevel(newLevel));
        yield return StartCoroutine(_fader.NormalFadeIn());
        EventManager.EventTrigger(EventType.FADING, true);
    }

    // Level to Menu Change Sequence
    IEnumerator LevelToMenu()
    {
        EventManager.EventTrigger(EventType.FADING, false);
        yield return StartCoroutine(_fader.NormalFadeOut());
        yield return StartCoroutine(UnloadLevel(_currentLevel.buildIndex));
        yield return StartCoroutine(UnloadScene(_gameplayIndex));
        yield return StartCoroutine(LoadScene(_mainMenuIndex));
        yield return StartCoroutine(_fader.NormalFadeIn());
        EventManager.EventTrigger(EventType.FADING, true);
    }

    // Menu to Level Change Sequence
    IEnumerator MenuToLevel(int levelSelected)
    {
        EventManager.EventTrigger(EventType.FADING, false);
        yield return StartCoroutine(_fader.NormalFadeOut());
        yield return StartCoroutine(UnloadScene(_mainMenuIndex));
        yield return StartCoroutine(LoadScene(_gameplayIndex));
        yield return StartCoroutine(LoadLevel(levelSelected));
        yield return StartCoroutine(_fader.NormalFadeIn());
        EventManager.EventTrigger(EventType.FADING, true);
    }

    // Only loads levels, does not load MainMenu scene or core scenes
    IEnumerator LoadLevel(int index)
    {
        yield return StartCoroutine(LoadScene(index));
        //EventManager.EventTrigger(EventType.LEVEL_STARTED, null);
        _currentLevel = SceneManager.GetSceneByBuildIndex(index);
    }

    // Only unloads levels, does not load MainMenu scene or core scenes
    IEnumerator UnloadLevel(int index)
    {
        //EventManager.EventTrigger(EventType.LEVEL_ENDED, null);
        yield return StartCoroutine(UnloadScene(index));
    }
    #endregion

    #region BUILD FUNCTIONALITY
    public int GetBuildIndex(string name)
    {
        for (int index = 0; index < _numOfScenes; index++)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(index));

            if (sceneName == name)
            {
                return index;
            }
        }

        Debug.LogError("Scene name not found");
        return -1;
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

    #region SCENE LOADING/UNLOADING FUNCTIONALITY
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
    #endregion

    #region EVENT HANDLERS
    // Starts PlayGame Coroutine
    public void PlayGameHandler(object data)
    {
        if (data is not int)
        {
            Debug.LogError("SceneSystemManager has not received a level num int!");
        }

        int levelNum = (int)data;

        // For now, start at level 1 when play is clicked in main menu
        if (levelNum != 1)
        {
            Debug.LogError("For now, we're starting at level 1 when play button is clicked.");
        }

        int levelIndex = levelNum + 2; // First level is actually index 3 in build
        StartCoroutine(MenuToLevel(levelIndex));
    }

    // Starts QuitGame Coroutine
    public void QuitGameHandler(object data)
    {
        StartCoroutine(QuitGame());
    }

    public void MainMenuHandler(object data)
    {
        StartCoroutine(LevelToMenu());
    }
    #endregion
}
