using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 5;
    public int currentHP = 5;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public GameObject heartPrefab; // Image с сердцем
    public Transform healthBar;     // контейнер

    private List<GameObject> hearts = new List<GameObject>();

    private PlayerStamina ps;
    public int GetHealth => currentHP;
    private void Awake()
    {
        ps = GetComponent<PlayerStamina>();

        Init(maxHP);

        UpdateHealth();
    }

    public void Init(int maxHealth)
    {
        // очищаем старые сердца
        foreach (Transform child in healthBar)
            Destroy(child.gameObject);

        hearts.Clear();

        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heartGO = Instantiate(heartPrefab, healthBar);
            //Image img = heartGO.GetComponent<Image>();
            //img.sprite = fullHeart;
            hearts.Add(heartGO);
        }
    }
    public void UpdateHealth()
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            SpriteRenderer heartImage = hearts[i].GetComponent<SpriteRenderer>();

            if (i >= (currentHP))
            {
                heartImage.sprite = emptyHeart;
            }

            else
            {
                heartImage.sprite = fullHeart;
            }
        }
    }

    public void SetHealth(int health)
    {
        currentHP = Mathf.Clamp(health, 0, maxHP);
        UpdateHealth();

        if (currentHP == 0)
        {
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        SetHealth(currentHP - damage);
        ps.SetStamina(100);
    }

    private void Die()
    {
        GameManager.Instance.PlayerDied();
    }
}
