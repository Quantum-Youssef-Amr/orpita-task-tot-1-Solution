using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.IO.Compression;

public class astro : MonoBehaviour, IDamageable
{
    [SerializeField] private float health = 100f, ImpulseForce = 5f, selfDamageMultiplier = 0.5f, DistanceToDespawn = 20f;
    [SerializeField] private int numberOfAsteroids = 2;
    [SerializeField] private GameObject[] Smaller_astro;
    [SerializeField] private GameObject ParticleSystem;
    [SerializeField] private int stage;
    [SerializeField] private AudioClip DieAudio;
    private const float TimeToDie = 5f;

    void Start()
    {
        StartCoroutine(CheckPlayerDist());
    }

    public void Damage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die(stage);
        }
    }

    public void Die(int _stage)
    {
        if (_stage != 0)
        {
            SpawnSmallerAsteroids();
        }
        GameObject m_particles = Instantiate(ParticleSystem, transform.position, Quaternion.identity);
        m_particles.transform.localScale = 0.5f * (_stage + 1) * Vector3.one;

        AudioManager.Instance.PlayerSfx(DieAudio);

        GameEventHandler.Instance.OnAstroDestroy?.Invoke(_stage);
        GameEventHandler.Instance.ScreenShake?.Invoke(_stage);

        Destroy(gameObject);
    }

    private void SpawnSmallerAsteroids()
    {
        for (int i = 0; i < numberOfAsteroids; i++)
        {
            GameObject m_astroid = Instantiate(Smaller_astro[Random.Range(0, Smaller_astro.Length)], transform.position, Quaternion.identity, transform.parent);

            Rigidbody2D m_astroRb = m_astroid.GetComponent<Rigidbody2D>();

            m_astroRb.AddForce(ImpulseForce * stage * Random.insideUnitCircle.normalized, ForceMode2D.Impulse);
            m_astroRb.AddTorque((Random.value - 0.5f) * ImpulseForce, ForceMode2D.Impulse);
        }
    }

    void OnCollisionEnter2D(Collision2D co)
    {
        if (co.collider.gameObject.CompareTag("Player"))
        {
            Rigidbody2D m_thisAstroidRB = GetComponent<Rigidbody2D>();
            co.collider.gameObject.GetComponent<IDamageable>().Damage(m_thisAstroidRB.mass * selfDamageMultiplier);

            if (stage == 0 && m_thisAstroidRB.linearVelocity.magnitude > 1f)
            {
                Die(stage);
                GameEventHandler.Instance.ScreenShake?.Invoke(1);
            }

        }
    }

    private Coroutine delete;
    private IEnumerator CheckPlayerDist()
    {
        if (IsPlayerInRange())
            delete ??= StartCoroutine(DeleteAfter(TimeToDie));

        else
        {
            if (delete != null)
            {
                StopCoroutine(delete);
                delete = null;
            }
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(CheckPlayerDist());
    }

    private bool IsPlayerInRange()
    {
        return Vector2.Distance(PlayerMovement.PlayerInstance.transform.position, transform.position) > DistanceToDespawn;
    }

    private IEnumerator DeleteAfter(float time)
    {
        yield return new WaitForSeconds(time);

        // if astro between distance updates and tries to delete this will abort the deletion
        if (IsPlayerInRange()) yield return 0;

        GameEventHandler.Instance.OnForceAstroDestroy?.Invoke();
        Destroy(gameObject);
    }
}
