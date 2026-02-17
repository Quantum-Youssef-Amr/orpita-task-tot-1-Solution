using UnityEngine;

public class ImageWiggle : MonoBehaviour
{
    [SerializeField] private float Intensity = 0.1f, shift = 0;

    private RectTransform _t;
    private Vector2 _startPos;
    void Start()
    {
        _t = GetComponent<RectTransform>();
        _startPos = _t.localPosition;
    }
    void Update()
    {
        _t.localPosition = _startPos + Intensity * new Vector2(Mathf.Sin(Time.time + shift), Mathf.Cos(Time.time + shift));
    }

}
