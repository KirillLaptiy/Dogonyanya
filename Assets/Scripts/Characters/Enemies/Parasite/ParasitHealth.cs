using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters.Allies.Akula;

public class ParasitHealth : MonoBehaviour
{
    public int maxHP = 2;
    public int currentHP = 2;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public GameObject heartPrefab; // Image ? ???????
    public Transform healthBar;     // ?????????

    public float attackCooldown = 3f;


    private float lastAttackTime = -999f;

    private ParasitFlee parasitFlee;

    private List<GameObject> hearts = new List<GameObject>();
    public int GetHealth => currentHP;
    private Coroutine speedBoostRoutine;

    private void Awake()
    {
        parasitFlee = GetComponent<ParasitFlee>();
        Init(maxHP);
        UpdateHealth();
    }

    public void Init(int maxHealth)
    {
        // ??????? ?????? ??????
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
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    void StartSpeedBoost()
    {
        if (speedBoostRoutine != null)
            StopCoroutine(speedBoostRoutine);

        speedBoostRoutine = StartCoroutine(SpeedBoostCoroutine());
    }

    IEnumerator SpeedBoostCoroutine()
    {
        parasitFlee.IncreaseSpeed(3f);

        yield return new WaitForSeconds(attackCooldown);

        parasitFlee.DecreaseSpeed(3f);

        speedBoostRoutine = null;
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        // ????????? ???
        if (!other.CompareTag("Player"))
            return;

        // ????????? ???????
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        // ??????? ????
        if (other.TryGetComponent(out PlayerHealth _) || other.TryGetComponent(out AkulaHealth _))
        {
            TakeDamage(1);
            StartSpeedBoost();
        }

        lastAttackTime = Time.time;
    }
}
