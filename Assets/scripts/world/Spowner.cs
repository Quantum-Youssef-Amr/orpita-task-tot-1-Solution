using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    [SerializeField] private float SpawnForce = 10f;
    [SerializeField] private GameObject[] Asteroids;
    [SerializeField] private Vector2Int Rate = new(2, 15);
    [SerializeField] private int HardLimit = 100;

    private float _rate;
    private int _spawned;
    private float _hardness = 1;

    void Start()
    {
        GameEventHandler.Instance.OnAstroDestroy += stage => CalcSpawned(stage);
        GameEventHandler.Instance.OnForceAstroDestroy += RemoveDeletedAsteroids;

        _rate = Rate.x;
        StartCoroutine(Spawn());
    }

    void OnEnable()
    {
        GameEventHandler.Instance.OnAstroDestroy += stage => CalcSpawned(stage);
        GameEventHandler.Instance.OnForceAstroDestroy += RemoveDeletedAsteroids;
    }

    void OnDisable()
    {
        GameEventHandler.Instance.OnAstroDestroy -= stage => CalcSpawned(stage);
        GameEventHandler.Instance.OnForceAstroDestroy -= RemoveDeletedAsteroids;
    }

    void OnDestroy()
    {
        GameEventHandler.Instance.OnAstroDestroy -= stage => CalcSpawned(stage);
        GameEventHandler.Instance.OnForceAstroDestroy -= RemoveDeletedAsteroids;
    }

    private void RemoveDeletedAsteroids() => _spawned--;

    private void CalcSpawned(int stage)
    {
        // the stage zero doesn't spawn astro so we dont count it with the spawning stages
        if (stage == 0)
            return;

        _spawned++;
        _hardness += 0.02f;
    }

    private IEnumerator Spawn()
    {
        if (_spawned < HardLimit)
        {
            Vector2 m_SpawnDirection = Random.insideUnitCircle;
            m_SpawnDirection.Normalize();
            Vector2 m_SpawnPos = (Vector2)PlayerMovement.PlayerInstance.transform.position + m_SpawnDirection * (Camera.main.orthographicSize * 2 + 5);

            GameObject m_astroid = Instantiate(
                Asteroids[Random.Range(0, Asteroids.Length - 1)],
                m_SpawnPos,
                Quaternion.identity,
                transform);

            m_astroid.GetComponent<Rigidbody2D>().AddForce(
                Random.value * SpawnForce * (0.7f * ((Vector2)PlayerMovement.PlayerInstance.transform.position - m_SpawnPos).normalized + 0.3f * Random.insideUnitCircle.normalized).normalized,
                ForceMode2D.Impulse);

            m_astroid.GetComponent<Rigidbody2D>().AddTorque(
                (Random.value - 0.5f) * SpawnForce,
                ForceMode2D.Impulse);

            _spawned++;
        }
        yield return new WaitForSeconds(1f / Mathf.Clamp(_rate * _hardness, Rate.x, Rate.y));
        StartCoroutine(Spawn());
    }
}

