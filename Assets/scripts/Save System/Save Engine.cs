using System.IO;
using System.Text;
using NUnit.Framework;
using UnityEngine;

public class SaveEngine : MonoBehaviour
{
    public bool useEncryption = false;
    public static SaveEngine Instance { get; private set; }
    public Data Data { get; set; }

    private string PATH;
    private readonly string SaveFile = "Save.ysa";
    private readonly string EncryptionKey = "YoussefAmr";

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("there is more than one Save engine");
            return;
        }
        Instance = this;
        PATH = Application.persistentDataPath;
        LoadData();
    }

    public void NewGame()
    {
        Data = new();
    }

    public void SaveData()
    {
        string m_saveFullPath = Path.Combine(PATH, SaveFile);
        string m_saveData = JsonUtility.ToJson(Data);
        string m_EncryptedData = useEncryption ? EncryptDecryptData(m_saveData) : m_saveData;

        using (FileStream stream = new(m_saveFullPath, FileMode.Create))
        {
            using (StreamWriter writer = new(stream))
            {
                writer.Write(m_EncryptedData);
            }
        }

    }

    public void LoadData()
    {
        string m_saveFullPath = Path.Combine(PATH, SaveFile);
        if (!File.Exists(m_saveFullPath))
        {
            NewGame();
            return;
        }

        string m_loadedEncryptedData = "";
        using (FileStream stream = new(m_saveFullPath, FileMode.Open))
        {
            using (StreamReader reader = new(stream))
            {
                m_loadedEncryptedData = reader.ReadToEnd();
            }
        }

        string m_decryptedData = useEncryption ? EncryptDecryptData(m_loadedEncryptedData) : m_loadedEncryptedData;
        Data m_loadedData = JsonUtility.FromJson<Data>(m_decryptedData);

        if (m_loadedData == null)
        {
            NewGame();
            return;
        }

        Data = m_loadedData;
    }

    public bool IsSaved()
    {
        return File.Exists(Path.Join(PATH, SaveFile));
    }

    private string EncryptDecryptData(string EncryptedData)
    {
        if (string.IsNullOrEmpty(EncryptionKey))
            return EncryptedData;

        StringBuilder m_sb = new();
        for (int i = 0, k = 0; i < EncryptedData.Length; i++, k++)
        {
            m_sb.Append((char)(EncryptedData[i] ^ EncryptionKey[k % EncryptionKey.Length]));
        }

        return m_sb.ToString();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveData();
    }

    void OnApplicationQuit()
    {
        SaveData();
    }
}
