using UnityEngine;

[System.Serializable]
public class Data
{
    public Settings settings;
    public int HighestScore;

    public Data()
    {
        settings = new();
        HighestScore = 0;
    }
}

[System.Serializable]
public class Settings
{
    public Platform Platform { get { return Application.isMobilePlatform ? Platform.Android : Platform.PC; } }
    public bool UsePostProcess = true;
    public bool UseMusic = true;
    public bool UseSFX = true;
}

public enum Platform
{
    PC,
    Android
}