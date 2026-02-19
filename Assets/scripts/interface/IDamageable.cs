using UnityEngine;

public interface IDamageable
{

    void Damage(float damage);
    void Die(int stage = 0);

}
