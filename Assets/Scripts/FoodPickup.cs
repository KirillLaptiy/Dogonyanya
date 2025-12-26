using UnityEngine;

public class FoodPickup : MonoBehaviour
{
    public int healAmount = 1;

    private void OnTriggerEnter2D(Collider2D character)
    {
        bool isPlayer = character.CompareTag("Player");
        bool canDestroy = false;

        if (isPlayer && character.TryGetComponent(out PlayerStamina stamina))
        {
            stamina.AddStamina(20);
            canDestroy = true;
        }

        if (canDestroy) Destroy(gameObject);
    }
}
