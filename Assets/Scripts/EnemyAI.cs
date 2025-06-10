using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Referensi Objek")]
    public Transform player;
    public Transform groundCheck;
    public Transform ledgeCheck;
    public LayerMask groundLayer;

    [Header("Pengaturan Gerak")]
    public float chaseSpeed = 2.5f;
    public float jumpForce = 10f; // Kita naikkan sedikit untuk coba

    // Komponen internal
    private Rigidbody2D rb;

    // Status
    private bool isGrounded;
    private bool isAtLedge;
    private float directionToPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        isAtLedge = !Physics2D.OverlapCircle(ledgeCheck.position, 0.1f, groundLayer);

        if (player != null)
        {
            directionToPlayer = Mathf.Sign(player.position.x - transform.position.x);
        }

        if (directionToPlayer != 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * directionToPlayer, transform.localScale.y, transform.localScale.z);
        }
    }

    void FixedUpdate()
    {
        if (isGrounded)
        {
            bool playerIsHigher = (player.position.y > transform.position.y + 1.5f);
            bool shouldJump = isAtLedge || playerIsHigher;

            if (shouldJump)
            {
                // JIKA HARUS LOMPAT:
                rb.linearVelocity = new Vector2(directionToPlayer * chaseSpeed, jumpForce); // <--- PAKAI .velocity
            }
            else
            {
                // JIKA TIDAK PERLU LOMPAT:
                rb.linearVelocity = new Vector2(directionToPlayer * chaseSpeed, rb.linearVelocity.y); // <--- PAKAI .velocity
            }
        }
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
        }
        if (ledgeCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(ledgeCheck.position, 0.1f);
        }
    }
}