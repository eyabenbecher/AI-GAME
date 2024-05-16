using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using UnityEngine.AI;

[System.Serializable]
public class Hide1 : ActionNode
{
    public LayerMask obstacleMask; // Layer mask to detect obstacles
    public float searchRadius = 10f; // Radius within which the guard searches for hiding spots
    public float hideSpeed = 5f; // Speed at which the guard moves to the hiding spot

    private Vector3 hidePosition; // Position of the chosen hiding spot
    private bool isHiding = false; // Flag to indicate if the guard is currently hiding

    protected override void OnStart()
    {
        // Start the search for a hiding spot
        FindHidePosition();
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        // If the guard hasn't found a hiding spot yet, continue searching
        if (!isHiding)
        {
            FindHidePosition();
            return State.Running;
        }

        // Move towards the hiding spot
        MoveToHidePosition();

        // If the guard has reached the hiding spot, consider it a success
        if (Vector3.Distance(context.transform.position, hidePosition) < 0.5f)
        {
            return State.Success;
        }

        return State.Running;
    }

    private void FindHidePosition()
    {
        // Generate random points within the search radius and check if they are suitable hiding spots
        Vector3 randomDirection = Random.insideUnitSphere * searchRadius;
        randomDirection += context.transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, searchRadius, NavMesh.AllAreas))
        {
            if (!Physics.Raycast(hit.position, Vector3.up, 2f, obstacleMask))
            {
                hidePosition = hit.position;
                isHiding = true;
            }
        }
    }

    private void MoveToHidePosition()
    {
        // Move the guard towards the hiding spot
        Vector3 direction = hidePosition - context.transform.position;
        context.transform.rotation = Quaternion.LookRotation(direction);
        context.transform.Translate(Vector3.forward * hideSpeed * Time.deltaTime);
    }
}
