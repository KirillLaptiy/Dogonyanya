using System;
using System.Collections.Generic;
using Characters.Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 5;
    public int currentHP = 5;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public AudioClip hitClip;

    public GameObject heartPrefab; // Image � �������
    public Transform healthBar;     // ���������

    private List<Image> hearts = new List<Image>();
    private AudioSource source;

    private PlayerStamina ps;
    public int GetHealth => currentHP;
    private void Awake()
    {
        ps = GetComponent<PlayerStamina>();
        source = GetComponent<AudioSource>();

        Init(maxHP);

        UpdateHealth();
    }

    public void Init(int maxHealth)
    {
        // Удаляем старые сердца
        foreach (Transform child in healthBar)
            Destroy(child.gameObject);

        hearts.Clear();

        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heartGO = Instantiate(heartPrefab, healthBar);
            RectTransform rt = heartGO.GetComponent<RectTransform>();

            rt.localScale = Vector3.one;
            rt.sizeDelta = new Vector2(50, 50); // размер сердца в UI
            rt.anchoredPosition = new Vector2(i * 55, 0); // горизонтальный ряд, расстояние 55 px

            Image img = heartGO.GetComponent<Image>();
            img.sprite = fullHeart;

            hearts.Add(img);
        }
    }

    public void UpdateHealth()
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i >= currentHP)
                hearts[i].sprite = emptyHeart;
            else
                hearts[i].sprite = fullHeart;
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
        source.PlayOneShot(hitClip, 0.7f);
        SetHealth(currentHP - damage);
        ps.SetStamina(100);
    }

    private void Die()
    {
        GameManager.Instance.PlayerDied();
    }
}
