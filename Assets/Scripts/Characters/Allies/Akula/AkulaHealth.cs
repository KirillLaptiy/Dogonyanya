using System;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.Allies.Akula
{
    public class AkulaHealth : MonoBehaviour
    {
        public int maxHP = 2;
        public int currentHP = 2;
        public Sprite fullHeart;
        public Sprite emptyHeart;

        public GameObject heartPrefab; // Image с сердцем
        public Transform healthBar; // контейнер

        private List<GameObject> hearts = new List<GameObject>();
        public int GetHealth => currentHP;

        private void Awake()
        {
            Init(maxHP);
            UpdateHealth();
        }

        public void Init(int maxHealth)
        {
            // очищаем старые сердца
            foreach (Transform child in healthBar)
                Destroy(child.gameObject);

            hearts.Clear();

            for (var i = 0; i < maxHealth; i++)
            {
                var heartGo = Instantiate(heartPrefab, healthBar);
                //Image img = heartGO.GetComponent<Image>();
                //img.sprite = fullHeart;
                hearts.Add(heartGo);
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
        }

        private void Die()
        {
            Destroy(gameObject);
        }
    }
}