using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("Referensi Objek")]
    public Transform player;
    public LayerMask groundLayer;

    [Header("Pengaturan Pemeriksaan Lingkungan")]
    public Transform groundCheck;
    public Transform wallCheck;
    public float groundCheckRadius = 0.2f;
    public float wallCheckDistance = 0.5f;

    [Header("Pengaturan Gerak & Status")]
    public float chaseSpeed = 3f;
    public float jumpForce = 12f;
    public float jumpHeightThreshold = 1.5f;
    public int damage = 1;
    public int maxHealth = 3;
    
    [Header("Loot")]
    public List<LootItem> lootTable = new List<LootItem>();
    
    // Komponen Internal & Status
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private int currentHealth;
    private Color originalColor;
    private bool isGrounded;
    private bool isFacingRight = true;
    private bool isWallAhead;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        originalColor = spriteRenderer.color;

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player Transform tidak di-assign DAN tidak ditemukan!", this);
                this.enabled = false;
            }
        }
    }

    void Update()
    {
        if (player == null) return;
        
        CheckSurroundings();
        HandleFlipping();
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float directionToPlayerX = Mathf.Sign(player.position.x - transform.position.x);

        if (isGrounded)
        {
            bool playerIsHigher = player.position.y > transform.position.y + jumpHeightThreshold;
            bool shouldJumpOverObstacle = isWallAhead;

            if (playerIsHigher || shouldJumpOverObstacle)
            {
                rb.linearVelocity = new Vector2(directionToPlayerX * chaseSpeed, jumpForce);
            }
            else
            {
                rb.linearVelocity = new Vector2(directionToPlayerX * chaseSpeed, rb.linearVelocity.y);
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(directionToPlayerX * chaseSpeed * 0.8f, rb.linearVelocity.y);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        StartCoroutine(FlashWhite());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashWhite()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    public void Die()
    {
        foreach (LootItem lootItem in lootTable)
        {
            if (UnityEngine.Random.Range(0f, 100f) <= lootItem.dropChance)
            {
                InstantiateLoot(lootItem.itemPrefab);
            }
        }
        Destroy(gameObject);
    }

    void InstantiateLoot(GameObject lootPrefab)
    {
        if (lootPrefab != null)
        {
            GameObject droppedLoot = Instantiate(lootPrefab, transform.position, Quaternion.identity);
            SpriteRenderer lootRenderer = droppedLoot.GetComponent<SpriteRenderer>();
            if (lootRenderer != null)
            {
                lootRenderer.color = Color.red;
            }
        }
    }
    
    #region Helper Methods
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D wallHit = Physics2D.Raycast(wallCheck.position, direction, wallCheckDistance, groundLayer);
        isWallAhead = wallHit.collider != null;
    }

    private void HandleFlipping()
    {
        if (player == null) return;

        if (player.position.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (player.position.x < transform.position.x && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
    
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Vector3 lineDirection = isFacingRight ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + lineDirection * wallCheckDistance);
        }
    }
    #endregion
}