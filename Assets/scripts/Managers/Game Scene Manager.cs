using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }
    [SerializeField] private CanvasGroup Overlay;
    private Coroutine _transitionInProgress;

    void Awake()
    {
        if (Instance != null)
            return;
        Instance = this;
    }

    void Start()
    {
        Instance.TransitionWithAddScene("MainMenu", 0, false);
    }

    /// <summary>
    /// Transition Scenes with unloading the current scene
    /// </summary>
    /// <param name="fromScene">scene to unload</param>
    /// <param name="toScene">scene that will be transitioned to</param>
    /// <param name="Duration">time of transition</param>
    /// <param name="WithOverlay">will we use a screen overlay to hide transition</param>
    public void TransitionWithReplaceScene(string fromScene, string toScene, float Duration, bool WithOverlay)
    {
        if (_transitionInProgress != null) return;
        SceneTransition m_transition = new SceneTransition(Overlay);

        m_transition.OnComplete += () => _transitionInProgress = null;

        _transitionInProgress ??= StartCoroutine(m_transition.Transition(toScene, fromScene).SetDuration(Duration).WithOverlay(WithOverlay).Preform());

    }

    /// <summary>
    /// Transition Scene with adding the new scene on top of the active scene
    /// </summary>
    /// <param name="toScene">scene that will be transitioned to</param>
    /// <param name="Duration">time of transition</param>
    /// <param name="WithOverlay">will we use a screen overlay to hide transition</param>
    public void TransitionWithAddScene(string toScene, float Duration, bool WithOverlay)
    {
        if (_transitionInProgress != null) return;
        SceneTransition m_transition = new SceneTransition(Overlay);

        m_transition.OnComplete += () => _transitionInProgress = null;

        _transitionInProgress ??= StartCoroutine(m_transition.Transition(toScene).SetDuration(Duration).WithOverlay(WithOverlay).Preform());
    }

}

public class SceneTransition
{

    private bool _withOverlay;
    private string _fromScene, _toScene;
    private float _transitionDuration;

    private CanvasGroup _overlay;
    public Action OnComplete;


    public SceneTransition(CanvasGroup Overlay)
    {
        _withOverlay = false;
        _fromScene = "";
        _toScene = "";
        _transitionDuration = 1;
        _overlay = Overlay;
    }

    public SceneTransition WithOverlay(bool Overlay = true)
    {
        _withOverlay = Overlay;
        return this;
    }

    public SceneTransition Transition(string to, string from = "")
    {
        _fromScene = from;
        _toScene = to;

        return this;
    }

    public SceneTransition SetDuration(float Duration)
    {
        _transitionDuration = Duration;
        return this;
    }

    public IEnumerator Preform()
    {
        if (!_withOverlay)
            _overlay.alpha = 0;

        if (_withOverlay)
            yield return ShowOverlay();

        Time.timeScale = 0;
        UnloadFromScene();
        yield return new WaitForSecondsRealtime(_transitionDuration / 2f);
        LoadToScene();
        Time.timeScale = 1;

        if (_withOverlay)
            yield return HideOverlay();

        OnComplete?.Invoke();
    }

    private IEnumerator HideOverlay()
    {
        yield return new WaitUntil(() =>
        {
            // Mathf.Max(_transitionDuration, 0.01f) if the _d == 0 there will be an error so we added the MAX
            _overlay.alpha = Mathf.Lerp(_overlay.alpha, _overlay.alpha - (1f / (Mathf.Max(_transitionDuration, 0.01f) / 4)), Time.deltaTime * 2);
            return _overlay.alpha < 0.05;
        });
        _overlay.alpha = 0;
    }

    private void LoadToScene()
    {
        AsyncOperation m_loadingScene = SceneManager.LoadSceneAsync(_toScene, LoadSceneMode.Additive);
        m_loadingScene.completed += (progress) =>
        {
            m_loadingScene.allowSceneActivation = true;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_toScene));
        };
    }

    private void UnloadFromScene()
    {
        if (_fromScene == "") return;

        AsyncOperation m_unloadedScene = SceneManager.UnloadSceneAsync(_fromScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        m_unloadedScene.completed += (progress) =>
        {
            m_unloadedScene.allowSceneActivation = true;
            Resources.UnloadUnusedAssets();
        };
    }

    private IEnumerator ShowOverlay()
    {
        yield return new WaitUntil(() =>
        {
            // Mathf.Max(_transitionDuration, 0.01f) if the _d == 0 there will be an error so we added the MAX
            _overlay.alpha = Mathf.Lerp(_overlay.alpha, _overlay.alpha + (1f / (Mathf.Max(_transitionDuration, 0.01f) / 4)), Time.deltaTime * 2);
            return _overlay.alpha > 0.95;
        });
        _overlay.alpha = 1;
    }
}
