using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float Health;
    [SerializeField] private AudioClip DamageAudio, DieAudio;

    public void Damage(float damage)
    {
        AudioManager.Instance.PlayerSfx(DamageAudio);
        Health -= damage;
        if (Health <= 0)
        {
            Die(0);
        }
        GameEventHandler.Instance.OnPlayerTakeDamage?.Invoke(Health);
    }

    public void Die(int _)
    {
        GameEventHandler.Instance.OnGameOver?.Invoke();
        AudioManager.Instance.PlayerSfx(DieAudio);
    }
}
