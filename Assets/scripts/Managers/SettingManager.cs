using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance { get; private set; }
    [SerializeField] private AudioMixer mixer;
    private Camera _main;

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        SceneManager.activeSceneChanged += (fromScene, ToScene) => applyGraphicsSettingsToNewScene(ToScene);
    }

    void Start()
    {
        applyAllSettings();
    }

    private void applyGraphicsSettingsToNewScene(Scene toScene)
    {
        _main = toScene.GetRootGameObjects().Where(gameObject => gameObject.CompareTag("MainCamera")).First().GetComponent<Camera>();
        _main.GetUniversalAdditionalCameraData().renderPostProcessing = SaveEngine.Instance.Data.settings.UsePostProcess;
    }

    public void applyAllSettings()
    {
        applyAudioSettings();
        applyGraphicsSettings();
    }

    private void applyGraphicsSettings()
    {
        _main.GetUniversalAdditionalCameraData().renderPostProcessing = SaveEngine.Instance.Data.settings.UsePostProcess;
    }

    public void ToggleGraphicsUse()
    {
        SaveEngine.Instance.Data.settings.UsePostProcess = !SaveEngine.Instance.Data.settings.UsePostProcess;
        applyGraphicsSettings();
    }

    private void applyAudioSettings()
    {
        mixer.SetFloat("sfxVolume", SaveEngine.Instance.Data.settings.UseSFX ? 0f : -80f);

        mixer.SetFloat("musicVolume", SaveEngine.Instance.Data.settings.UseMusic ? 0f : -80f);
    }

    public void ToggleSfxUse()
    {
        SaveEngine.Instance.Data.settings.UseSFX = !SaveEngine.Instance.Data.settings.UseSFX;
        applyAudioSettings();
    }

    public void ToggleMusicUse()
    {
        SaveEngine.Instance.Data.settings.UseMusic = !SaveEngine.Instance.Data.settings.UseMusic;
        applyAudioSettings();
    }
}
