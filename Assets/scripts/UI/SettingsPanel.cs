using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private Sprite[] MusicSprites;
    [SerializeField] private Sprite[] SoundSprites;
    [SerializeField] private Sprite[] GraphicsSprites;

    [SerializeField] private Image MusicSprite, SoundSprite, GraphicsSprite;

    [SerializeField] private AudioMixer audioMixer;

    public void OnSettingOpen()
    {
        MusicSprite.sprite = MusicSprites[SaveEngine.Instance.Data.settings.UseMusic ? 1 : 0];
        SoundSprite.sprite = SoundSprites[SaveEngine.Instance.Data.settings.UseSFX ? 1 : 0];
        GraphicsSprite.sprite = GraphicsSprites[SaveEngine.Instance.Data.settings.UsePostProcess ? 1 : 0];
    }

    public void ToggleMusic()
    {
        SettingManager.Instance.ToggleMusicUse();

        // changing btn sprite
        MusicSprite.sprite = MusicSprites[SaveEngine.Instance.Data.settings.UseMusic ? 1 : 0];


    }

    public void ToggleSound()
    {
        SettingManager.Instance.ToggleSfxUse();
        // changing btn sprite
        SoundSprite.sprite = SoundSprites[SaveEngine.Instance.Data.settings.UseSFX ? 1 : 0];
    }

    public void ToggleGraphics()
    {
        SettingManager.Instance.ToggleGraphicsUse();
        // changing btn sprite
        GraphicsSprite.sprite = GraphicsSprites[SaveEngine.Instance.Data.settings.UsePostProcess ? 1 : 0];
    }
}
