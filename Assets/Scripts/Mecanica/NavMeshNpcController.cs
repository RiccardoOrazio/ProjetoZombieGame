using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshNpcController : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float timeBetweenWanders = 5f;

    [Header("Object References")]
    [SerializeField] private Transform spriteTransform;
    [SerializeField] private bool spriteFacesRightByDefault = true;

    private NavMeshAgent agent;
    private float wanderTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;

        if (spriteTransform == null)
        {
            Debug.LogError("A referência para o 'Sprite Transform' não foi definida no Inspector!");
            enabled = false;
        }
    }

    void Update()
    {
        wanderTimer += Time.deltaTime;

        if (wanderTimer >= timeBetweenWanders)
        {
            Vector3 newPos = GetRandomNavMeshPosition(transform.position, wanderRadius);
            agent.SetDestination(newPos);
            wanderTimer = 0f;
        }

        HandleSpriteDirection();
    }

    private void HandleSpriteDirection()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            float horizontalMovement = agent.velocity.x;

            if (Mathf.Abs(horizontalMovement) > 0.01f)
            {
                float directionMultiplier = spriteFacesRightByDefault ? 1f : -1f;
                float scaleX = Mathf.Abs(spriteTransform.localScale.x);

                spriteTransform.localScale = new Vector3(
                    scaleX * Mathf.Sign(horizontalMovement) * directionMultiplier,
                    spriteTransform.localScale.y,
                    spriteTransform.localScale.z
                );
            }
        }
    }

    public static Vector3 GetRandomNavMeshPosition(Vector3 origin, float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, -1);
        return navHit.position;
    }
}