using UnityEngine;
using TheKiwiCoder;
using UnityEngine.AI;

[System.Serializable]
public class Patrolll : ActionNode
{
    private Vector3 lastKnownLocation; // The last known location of the player
    private float patrolTimer = 1.0f; // Timer for patrolling
    public float patrolInterval = 3.0f; // Interval between patrolling
    public float patrolDistance; // Maximum distance to patrol from last known location
    private GameObject guard;
    public guardProp guardProp;

    protected override void OnStart()
    {
        //Debug.Log("Patrolling...");
        lastKnownLocation = context.gameObject.transform.position;
        guard = context.gameObject;
        guardProp = guard.GetComponent<guardProp>();
        patrolDistance = guardProp.distance_patrouille;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        patrolTimer += Time.deltaTime;

        if (patrolTimer >= patrolInterval)
        {
            PatrolRandomLocation();
            patrolTimer = 0.0f;
        }

        return State.Running;
    }

    private void PatrolRandomLocation()
    {
        // Generate a random point within patrolDistance from the last known location
        Vector3 randomPoint = lastKnownLocation + Random.insideUnitSphere * patrolDistance;
        randomPoint.y = 0; // Ensure the point stays on the same level

        // Set the guard's destination to the random point
        guard.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(randomPoint);
    }
}
