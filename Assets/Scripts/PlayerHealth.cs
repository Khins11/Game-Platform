using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    public HealthUI healthUI; // <-- Variabel untuk referensi ke UI

    private SpriteRenderer spriteRenderer;

    public static event Action OnPlayedDied;

    void Start()
    {
        ResetHealth();
        spriteRenderer = GetComponent<SpriteRenderer>();
        GameController.OnReset += ResetHealth;
        HealthItem.OnHealthCollect += Heal;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Cek jika yang bertabrakan punya script EnemyAI
        if (collision.GetComponent<EnemyAI>())
        {
            // Untuk sementara, kita anggap setiap musuh memberi 1 damage
            // Ini karena script EnemyAI kita belum punya variabel 'damage'
            TakeDamage(1); // <-- DIUBAH DARI enemy.damage
        }
    }

    void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthUI.UpdateHearts(currentHealth);
    }

    void ResetHealth()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Panggil nama method yang benar
        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth); // <-- DIUBAH DARI .Update()
        }

        StartCoroutine(FlashRed());
        if (currentHealth <= 0)
        {
            Debug.Log("Player Mati!");
            // Di sini Anda bisa menambahkan logika untuk game over, restart level, dll.
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