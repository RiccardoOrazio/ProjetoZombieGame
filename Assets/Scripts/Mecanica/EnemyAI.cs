using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform playerTransform;
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
    [SerializeField] private float attackDamageDelay = 0.8f;
    [SerializeField] private float attackMoveLockTime = 1.0f;

    [Header("Configurações de Desvio")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float obstacleRayLength = 1.5f;
    [SerializeField] private float bodyRadius = 0.5f;

    [Header("Variação Visual")]
    [SerializeField] private bool isHeadless = false;

    private Rigidbody rb;
    private Animator animator;
    private Camera mainCamera;
    private float lastAttackTime = -999f;
    private Vector3 aimDirection = Vector3.forward;
    private bool shouldChase = false;
    private float distanceToPlayer;
    private bool isStunned = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;

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

        if (attackPoint == null)
        {
            Debug.LogError("IA do Inimigo: 'Attack Point' não definido!", this);
            enabled = false;
        }

        aimDirection = Vector3.forward;
    }

    void Start()
    {
        if (animator != null)
        {
            animator.SetLayerWeight(1, isHeadless ? 1f : 0f);
        }
    }

    public void SetStunned(bool state)
    {
        isStunned = state;
        if (isStunned)
        {
            shouldChase = false;
            rb.linearVelocity = Vector3.zero;
            if (animator != null) animator.SetBool("IsMoving", false);
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        if (isStunned) return;

        if (DialogueManager.instance != null && DialogueManager.instance.IsDialogueActive)
        {
            shouldChase = false;
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);

            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
            }
            return;
        }

        distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        bool isAttacking = Time.time < lastAttackTime + attackCooldown;
        bool isLockedInPlace = Time.time < lastAttackTime + attackMoveLockTime;

        if (isLockedInPlace)
        {
            shouldChase = false;
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            HandleAiming();
            PositionAttackPoint();
        }
        else if (distanceToPlayer <= detectionRadius)
        {
            HandleAiming();
            PositionAttackPoint();

            if (distanceToPlayer <= attackRadius && !isAttacking)
            {
                shouldChase = false;
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
                HandleAttack();
            }
            else
            {
                shouldChase = true;
            }
        }
        else
        {
            shouldChase = false;
        }

        if (animator != null)
        {
            bool isMoving = rb.linearVelocity.magnitude > 0.1f;
            animator.SetBool("IsMoving", isMoving);

            var forward = mainCamera.transform.forward;
            forward.y = 0;
            forward.Normalize();
            var right = mainCamera.transform.right;
            right.y = 0;
            right.Normalize();

            float dirX = Vector3.Dot(aimDirection, right);
            float dirY = Vector3.Dot(aimDirection, forward);

            animator.SetFloat("DirX", dirX);
            animator.SetFloat("DirY", dirY);
        }
    }

    void FixedUpdate()
    {
        if (isStunned) return;

        if (shouldChase)
        {
            Vector3 idealDirection = aimDirection;
            Vector3 finalDirection = CalculateAvoidanceDirection(idealDirection);
            Vector3 targetVelocity = finalDirection * moveSpeed;
            rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    private Vector3 CalculateAvoidanceDirection(Vector3 idealDirection)
    {
        if (Physics.SphereCast(transform.position, bodyRadius, idealDirection, out RaycastHit hit, obstacleRayLength, obstacleLayer))
        {
            Vector3 slideDirection = Vector3.ProjectOnPlane(idealDirection, hit.normal);

            if (slideDirection.sqrMagnitude < 0.01f)
            {
                Vector3 contourDirection = Vector3.Cross(Vector3.up, hit.normal);
                return contourDirection.normalized;
            }

            Vector3 avoidanceForce = hit.normal * 0.1f;
            return (slideDirection + avoidanceForce).normalized;
        }

        return idealDirection;
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

            if (animator != null) animator.SetTrigger("Attack");

            StartCoroutine(ApplyDamageAfterDelay(attackDamageDelay));
        }
    }

    private IEnumerator ApplyDamageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (playerTransform == null) yield break;
        if (isStunned) yield break;

        float distanceToPlayerOnImpact = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayerOnImpact > attackRadius + 1.5f) yield break;

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

        if (mainCamera != null)
        {
            Vector3 finalDirection = CalculateAvoidanceDirection(aimDirection);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + finalDirection * obstacleRayLength);
            Gizmos.DrawWireSphere(transform.position + finalDirection * obstacleRayLength, bodyRadius);
        }
    }
}