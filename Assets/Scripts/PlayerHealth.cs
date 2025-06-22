using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public HealthUI healthUI;
    
    private int currentHealth;
    private SpriteRenderer spriteRenderer;

    public static event Action OnPlayedDied; 

    private void Awake()
    {
        // Baris GameController.OnReset dihapus dari sini
        HealthItem.OnHealthCollect += Heal;
    }

    private void OnDestroy()
    {
        // Baris GameController.OnReset dihapus dari sini
        HealthItem.OnHealthCollect -= Heal;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Method ResetHealth tetap dipanggil di sini, jadi kesehatan akan
        // selalu penuh setiap kali scene/game dimulai.
        ResetHealth();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<EnemyAI>())
        {
            TakeDamage(1);
        }
    }

    void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth); // Cara lebih aman untuk heal
        
        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }
    }

    void ResetHealth()
    {
        currentHealth = maxHealth;
        if (healthUI != null)
        {
            healthUI.SetMaxHearts(maxHealth);
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }

        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Debug.Log("Player Mati!");
            OnPlayedDied.Invoke();
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }
}