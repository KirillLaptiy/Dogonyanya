using Characters.Allies.Akula;
using UnityEngine;

public class VoenkomHitCharacter : MonoBehaviour
{
    [Header("Attack Settings")]
    public int damage = 1;
    public float attackCooldown = 2f;

    private float lastAttackTime = -999f;

    private VoenkomHealth vh;

    private bool isStunned = false;

    private void Awake()
    {
        vh = GetComponent<VoenkomHealth>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isStunned) return;

        // Проверяем тег
        if (!other.CompareTag("Player"))
            return;

        // Проверяем кулдаун
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        bool hitAlly = false;

        // Наносим урон
        if (other.TryGetComponent(out PlayerHealth health))
        {
            health.TakeDamage(damage);
        }

        if (other.TryGetComponent(out AkulaHealth aHealth))
        {
            aHealth.TakeDamage(damage);
            hitAlly = true;
        }

        lastAttackTime = Time.time;

        float stunAdditionalDuration = hitAlly ? 4f : 0;
        vh.ApplyStun(stunAdditionalDuration);
    }

    public void SetStunned(bool stunned)
    {
        isStunned = stunned;
    }
}
