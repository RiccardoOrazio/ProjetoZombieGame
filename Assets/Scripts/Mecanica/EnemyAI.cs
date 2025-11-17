using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform spriteTransform;
    [SerializeField] private Transform attackPoint;

    [Header("Configurações da IA")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float attackRadius = 2f;
    [SerializeField] private float attackPointDistance = 1.5f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Configurações de Ataque")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackHitboxRadius = 0.5f;
    [SerializeField] private float attackWindUpTime = 0.8f; // NOVO: Tempo de preparação antes de atacar

    [Header("Configurações do Sprite")]
    [SerializeField] private bool spriteFacesRightByDefault = true;

    private Rigidbody rb;
    private Animator animator;
    private float lastAttackTime = -999f;
    private Vector3 aimDirection = Vector3.forward;
    private bool shouldChase = false;
    private float distanceToPlayer;
    private float windUpTimer = 0f; // NOVO: Timer para o "wind-up"

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                Debug.LogError("IA do Inimigo: Player não encontrado! Verifique a tag 'Player'.", this);
                enabled = false;
            }
        }

        if (spriteTransform == null)
        {
            Debug.LogError("IA do Inimigo: 'Sprite Transform' não definido!", this);
            enabled = false;
        }
        if (attackPoint == null)
        {
            Debug.LogError("IA do Inimigo: 'Attack Point' não definido!", this);
            enabled = false;
        }

        aimDirection = spriteFacesRightByDefault ? Vector3.right : Vector3.left;
    }

    void Update()
    {
        if (playerTransform == null) return;

        distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRadius)
        {
            HandleAiming();
            PositionAttackPoint();
            HandleSpriteDirection();

            if (distanceToPlayer <= attackRadius)
            {
                // --- LÓGICA DE ATAQUE ATUALIZADA ---
                shouldChase = false;

                // 1. Começa a contar o tempo de preparação
                windUpTimer += Time.deltaTime;

                // 2. Só tenta atacar se o tempo de preparação terminou
                if (windUpTimer >= attackWindUpTime)
                {
                    HandleAttack(); // HandleAttack() agora só é chamado após o wind-up
                }
            }
            else
            {
                shouldChase = true;
                windUpTimer = 0f; // Reseta o timer de preparação se o player sair do raio
            }
        }
        else
        {
            shouldChase = false;
            windUpTimer = 0f; // Reseta o timer de preparação se o player sair do raio
        }

        if (animator != null)
        {
            bool isMoving = rb.linearVelocity.magnitude > 0.1f;
            animator.SetBool("IsMoving", isMoving);

            animator.SetFloat("DirX", aimDirection.x);
            animator.SetFloat("DirY", aimDirection.z);
        }
    }

    void FixedUpdate()
    {
        if (shouldChase)
        {
            Vector3 targetVelocity = aimDirection * moveSpeed;
            rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    private void HandleAiming()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position);
        directionToPlayer.y = 0;

        if (directionToPlayer.sqrMagnitude > 0.01f)
        {
            aimDirection = directionToPlayer.normalized;
        }
    }

    private void PositionAttackPoint()
    {
        Vector3 offset = aimDirection * attackPointDistance;

        attackPoint.position = new Vector3(
            transform.position.x + offset.x,
            attackPoint.position.y,
            transform.position.z + offset.z
        );

        if (aimDirection != Vector3.zero)
        {
            attackPoint.rotation = Quaternion.LookRotation(aimDirection);
        }
    }

    private void HandleAttack()
    {
        if (Time.time > lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            windUpTimer = 0f;

            if (animator != null) animator.SetTrigger("Attack");

            Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackHitboxRadius);

            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(attackDamage);
                        Debug.Log("Inimigo ACERTOU o Player!");
                        break;
                    }
                }
            }
        }
    }

    private void HandleSpriteDirection()
    {
        float horizontalDirection = 0f;

        if (rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            horizontalDirection = rb.linearVelocity.x;
        }
        else if (distanceToPlayer <= detectionRadius)
        {
            horizontalDirection = aimDirection.x;
        }

        if (Mathf.Abs(horizontalDirection) > 0.01f)
        {
            float directionMultiplier = spriteFacesRightByDefault ? 1f : -1f;
            float scaleX = Mathf.Abs(spriteTransform.localScale.x);

            spriteTransform.localScale = new Vector3(
                scaleX * Mathf.Sign(horizontalDirection) * directionMultiplier,
                spriteTransform.localScale.y,
                spriteTransform.localScale.z
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        if (attackPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(attackPoint.position, attackHitboxRadius);
        }
    }
}