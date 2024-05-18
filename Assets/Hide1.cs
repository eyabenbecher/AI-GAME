using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using UnityEngine.AI;

[System.Serializable]
public class Hide1 : ActionNode
{
    public Transform targetHidingSpot; // Specific hiding spot the NPC will go to
    public float stoppingDistance = 0.5f; // Distance to the hiding spot considered as reached
    public float hideSpeed = 15f; // Speed at which the NPC moves to the hiding spot

    private NavMeshAgent agent; // Reference to the NavMeshAgent component

    protected override void OnStart()
    {
        // Initialize the NavMeshAgent and set its destination to the hiding spot
        InitializeAgent();
        if (agent != null)
        {
            agent.speed = hideSpeed;
            agent.SetDestination(targetHidingSpot.position);
        }
    }

    protected override void OnStop()
    {
        // Stop the agent when the action is stopped
        if (agent != null)
        {
            agent.ResetPath();
        }
    }

    protected override State OnUpdate()
    {
        // If the NPC has reached the hiding spot, consider it a success
        if (agent == null)
        {
            return State.Failure; // Fail if the agent is not found
        }

        if (!agent.pathPending && agent.remainingDistance <= stoppingDistance)
        {
            return State.Success;
        }

        return State.Running;
    }

    private void InitializeAgent()
    {
        // Get the NavMeshAgent component from the context's GameObject
        agent = context.gameObject.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found on NPC.");
        }
    }
}
