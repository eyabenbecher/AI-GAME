using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Test : MonoBehaviour
{
    public Transform player;
    public float patrolSpeed = 5f;
    public float fleeSpeed = 15f;
    public float patrolRange = 50f;
    public float fleeDistance = 70f;
    public float patrolWaitTime = 1.0f;
    public float timeBetweenAttacks = 5;
    public bool alreadyAttacked;
    public float attackRange =15f;
    public bool playerInAttackRange;
    public GameObject projectile;
    public float shootingInterval = 3f; // Interval between shots
    public int maxBulletsPerReload = 12;

    [Header("what not")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float bulletSpeed = 40f;
    public float spawnInterval = 5;

    private Vector3 initialPosition;
    private float patrolTiming;
    private NavMeshAgent navMeshAgent;
    private float stuckTimer;
    private const float maxStuckTime = 2.0f;
    private bool isFleeing = false;
    private const int maxFleeAttempts = 10;

    private Unit unit; // Reference to the Unit script
    public float healAmount = 20f; // Amount to heal
    public float healInterval = 4f; // Interval between healing attempts
    private bool isHealing = false; // Whether the unit is currently healing
    private float lastShootTime;
    private int bulletsShot;

    private void Start()
    {
        initialPosition = transform.position;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = patrolSpeed;

        unit = GetComponent<Unit>(); // Get the Unit component
        StartCoroutine(CheckHealthAndHeal());

        lastShootTime = -shootingInterval; // Start with the last shoot time set to ensure immediate shooting
        bulletsShot = 0;
    }

    private void Update()
    {
        
        float health = unit.GetHealthPercentage() * 100;
        float distance = Vector3.Distance(transform.position, player.position);
        playerInAttackRange = distance <= attackRange;

        if (playerInAttackRange && !isHealing && !isFleeing)
        {
            if (Time.time - lastShootTime >= shootingInterval && bulletsShot < maxBulletsPerReload)
            {
                StartCoroutine(FireBalls());
            }
        }
        else
        {
            Patrol();
        }

        if (CalculateHealthAnxiety(health) > CalculateAnxiety(distance))
        {
            if (CalculateHealthAnxiety(health) > CalculateAmmoAnxiety(bulletsShot - 12))
            {
                // Heal if health anxiety is greater than both distance anxiety and ammo anxiety
                StartCoroutine(CheckHealthAndHeal());
            }
            else
            {
                // Reload if ammo anxiety is greater
                StartCoroutine(Reload());
            }
        }
        else
        {
            if (CalculateAnxiety(distance) > CalculateAmmoAnxiety(bulletsShot - 12))
            {
                // Flee if distance anxiety is greater than ammo anxiety
                Flee();
            }
            else
            {
                // Reload if ammo anxiety is greater
                StartCoroutine(Reload());
            }
        }

        
    }

    private IEnumerator CheckHealthAndHeal()
    {
        while (true)
        {
            if (unit.GetHealthPercentage() < 0.4f && !isHealing)
            {
                isHealing = true;
                navMeshAgent.isStopped = true;
                unit.Heal(healAmount);
                yield return new WaitForSeconds(healInterval);
                navMeshAgent.isStopped = false;
                isHealing = false;
            }
            yield return new WaitForSeconds(1f); // Check health every second
        }
    }

    private float CalculateAnxiety(float distance)
    {
        // Use a curve to calculate anxiety based on distance
        float anxiety = Mathf.Pow(50f - distance, 3) / Mathf.Pow(distance, 3);
        anxiety = Mathf.Clamp01(anxiety);
        return anxiety;
    }

    private float CalculateAmmoAnxiety(int ammo)
    {
        float anxiety = Mathf.Pow(12f - ammo, 2) / Mathf.Pow(12, 2);
        anxiety = Mathf.Clamp01(anxiety);
        return anxiety;
    }

    private float CalculateHealthAnxiety(float health)
    {
        float a = 1f;
        float b = -0.2f;
        float c = -40f;
        float d = 0f;
        // Use a curve to calculate anxiety based on health
        float anxiety = a / (1 + Mathf.Exp(-b * (health + c))) + d;
        anxiety = Mathf.Clamp01(anxiety);
        return anxiety;
    }

    private bool ShouldFlee(float anxiety)
    {
        float fleeProbability = Mathf.Clamp01(1 - Mathf.Pow(1 - anxiety, 2));
        return Random.value < fleeProbability;
    }

    private void Patrol()
    {
        if (isFleeing)
        {
            isFleeing = false;
            navMeshAgent.speed = patrolSpeed;
        }

        if (!navMeshAgent.hasPath || navMeshAgent.remainingDistance < 0.5f)
        {
            patrolTiming += Time.deltaTime;
            if (patrolTiming >= patrolWaitTime)
            {
                Vector3 patrolPoint = initialPosition + Random.insideUnitSphere * patrolRange;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(patrolPoint, out hit, patrolRange, NavMesh.AllAreas))
                {
                    navMeshAgent.SetDestination(hit.position);
                }
                patrolTiming = 0;
            }
        }
    }

    private void Flee()
    {
        if (!isFleeing)
        {
            isFleeing = true;
            navMeshAgent.speed = fleeSpeed;
        }

        bool validFleePositionFound = false;
        int fleeAttempts = 0;

        while (!validFleePositionFound && fleeAttempts < maxFleeAttempts)
        {
            fleeAttempts++;
            Vector3 fleeDirection = (transform.position - player.position).normalized;
            Vector3 fleePosition = transform.position + fleeDirection * fleeDistance;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(fleePosition, out hit, fleeDistance, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(hit.position);
                stuckTimer = 0;
                validFleePositionFound = true;
            }
            else
            {
                stuckTimer += Time.deltaTime;
                if (stuckTimer > maxStuckTime)
                {
                    patrolTiming = 0;
                    isFleeing = false;
                    Patrol();
                    return;
                }
            }
        }

        if (!validFleePositionFound)
        {
            patrolTiming = 0;
            isFleeing = false;
            Patrol();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, fleeDistance);
    }

    private void AttackPlayer()
    {
        // Make sure enemy doesn't move
        navMeshAgent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            // Attack code here
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), 12);
        }
    }

    private IEnumerator FireBalls()
    {
        while (playerInAttackRange)
        {
            FireBall();
            yield return new WaitForSeconds(timeBetweenAttacks);
        }
    }

    public void FireBall()
    {
        if (Time.time - lastShootTime >= shootingInterval && bulletsShot < maxBulletsPerReload)
        {

            // Calculate the direction from the enemy to the player
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            // Rotate the enemy to face the player
            transform.rotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
            // Instantiate bullet
            Rigidbody rb = Instantiate(bullet, spawnPoint.position, spawnPoint.rotation).GetComponent<Rigidbody>();

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
                StartCoroutine(Reload());
            }

            Destroy(rb.gameObject, 5f);
        }
    }

    private IEnumerator Reload()
    {
        // Wait for some time to simulate reloading
        yield return new WaitForSeconds(3f); // Reload time can be adjusted

        // Reset bullet counter
        bulletsShot = 0;
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
