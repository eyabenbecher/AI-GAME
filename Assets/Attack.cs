using System.Collections;
using UnityEngine;
using TheKiwiCoder;
using UnityEngine.AI;

[System.Serializable]
public class Attack : ActionNode
{
    public Transform player;
    public Transform spawnPoint;
    public GameObject bullet;
    public float bulletSpeed = 20f;
    public float timeBetweenAttacks = 1f;
    public float shootingInterval = 1f;
    public int maxBulletsPerReload = 5;

    private NavMeshAgent agent;
    private Coroutine attackCoroutine;
    private float lastShootTime;
    private int bulletsShot;
    private bool playerInAttackRange;

    private MonoBehaviour coroutineHandler; // Reference to MonoBehaviour to handle coroutines

    protected override void OnStart()
    {
        agent = context.gameObject.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found on NPC.");
            return;
        }

        // Get a MonoBehaviour reference from the context to start coroutines
        coroutineHandler = context.gameObject.GetComponent<MonoBehaviour>();
        if (coroutineHandler == null)
        {
            Debug.LogError("Context is not a MonoBehaviour. Coroutine handling requires MonoBehaviour.");
            return;
        }

        // Start the attack coroutine
        attackCoroutine = coroutineHandler.StartCoroutine(FireBalls());
    }

    protected override void OnStop()
    {
        // Stop the attack coroutine
        if (attackCoroutine != null)
        {
            coroutineHandler.StopCoroutine(attackCoroutine);
        }

        if (agent != null)
        {
            agent.ResetPath();
        }
    }

    protected override State OnUpdate()
    {
        // Logic to determine if the player is in attack range
        playerInAttackRange = Vector3.Distance(context.transform.position, player.position) <= agent.stoppingDistance;

        if (playerInAttackRange)
        {
            return State.Running;
        }
        else
        {
            return State.Failure;
        }
    }

    private IEnumerator FireBalls()
    {
        while (true)
        {
            if (playerInAttackRange)
            {
                FireBall();
                yield return new WaitForSeconds(timeBetweenAttacks);
            }
            else
            {
                yield return null;
            }
        }
    }

    public void FireBall()
    {
        if (Time.time - lastShootTime >= shootingInterval && bulletsShot < maxBulletsPerReload)
        {
            // Calculate the direction from the enemy to the player
            Vector3 directionToPlayer = (player.position - context.transform.position).normalized;

            // Rotate the enemy to face the player
            context.transform.rotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);

            // Instantiate bullet
            Rigidbody rb = Object.Instantiate(bullet, spawnPoint.position, spawnPoint.rotation).GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Ensure gravity is enabled for the projectile
                rb.useGravity = true;

                // Apply an initial velocity with an arc (upward force)
                Vector3 forceDirection = spawnPoint.forward + Vector3.up * 0.2f;
                rb.velocity = forceDirection * bulletSpeed;

                // Optionally, add a small random deviation to make it less perfect
                rb.velocity += new Vector3(
                    Random.Range(-0.1f, 0.1f),
                    Random.Range(-0.1f, 0.1f),
                    Random.Range(-0.1f, 0.1f)
                );
            }
            lastShootTime = Time.time;
            bulletsShot++;

            // Check if reload is needed
            if (bulletsShot >= maxBulletsPerReload)
            {
                coroutineHandler.StartCoroutine(Reload());
            }
        }
    }

    private IEnumerator Reload()
    {
        // Example reload logic, adjust as needed
        yield return new WaitForSeconds(2f); // Wait for 2 seconds to reload
        bulletsShot = 0; // Reset bullets shot count
    }
}
