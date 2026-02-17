using UnityEngine;

public class MobileControls : MonoBehaviour
{
    void Start() => gameObject.SetActive(SaveEngine.Instance.Data.settings.Platform == Platform.Android);
}
