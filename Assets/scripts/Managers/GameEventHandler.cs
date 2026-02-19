using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEventHandler : MonoBehaviour
{
    public static GameEventHandler Instance { get; private set; }

    public Action<int> ScreenShake;

    public Action<int> OnAstroDestroy;
    public Action OnForceAstroDestroy;

    public Action<float> OnPlayerTakeDamage;

    public Action<int> OnScoreChanged;

    public Action OnGameOver;
    public Action OnGamePaused;

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        SceneManager.activeSceneChanged += (_, _) => FreeOldReferences();
    }

    private void FreeOldReferences()
    {
        ScreenShake = null;

        OnAstroDestroy = null;
        OnForceAstroDestroy = null;

        OnPlayerTakeDamage = null;
        OnScoreChanged = null;

        OnGameOver = null;
        OnGamePaused = null;
    }
}
