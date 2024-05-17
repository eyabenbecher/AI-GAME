using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PatrolAndFleeBehavior : MonoBehaviour
{
    public Transform player;
    public float patrolSpeed = 3.5f;
    public float fleeSpeed = 8f;
    public float patrolRange = 20f;
    public float fleeDistance = 50f;
    public float patrolWaitTime = 5.0f;
    public Text stateText;

    private Vector3 initialPosition;
    private float patrolTiming;
    private NavMeshAgent navMeshAgent;
    private float stuckTimer;
    private const float maxStuckTime = 2.0f;
    private bool isFleeing = false;
    private const int maxFleeAttempts = 10;

    private void Start()
    {
        initialPosition = transform.position;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = patrolSpeed;
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        float anxiety = CalculateAnxiety(distance);

        if (ShouldFlee(anxiety))
        {
            Flee();
        }
        else
        {
            Patrol();
        }
    }

    private float CalculateAnxiety(float distance)
    {
        
        float anxiety = Mathf.Pow(50f - distance, 3) / Mathf.Pow(distance, 3);
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
                stateText.text = "Patrolling";
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
                stateText.text = "Fleeing";
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
}