using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using UnityEngine.AI;

[System.Serializable]
public class Hide1 : ActionNode
{
    public Transform targetHidingSpot; // Specific hiding spot the NPC will go to
    public float hideSpeed = 5f; // Speed at which the NPC moves to the hiding spot

    private bool isHiding = false; // Flag to indicate if the NPC is currently hiding

    protected override void OnStart()
    {
        // Start moving to the predefined hiding spot
        MoveToHidePosition();
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        // If the NPC has reached the hiding spot, consider it a success
        if (Vector3.Distance(context.transform.position, targetHidingSpot.position) < 0.5f)
        {
            return State.Success;
        }

        // Move towards the hiding spot
        MoveToHidePosition();

        return State.Failure;
    }

    private void MoveToHidePosition()
    {
        // Move the NPC towards the predefined hiding spot
        Vector3 direction = targetHidingSpot.position - context.transform.position;
        context.transform.rotation = Quaternion.LookRotation(direction);
        context.transform.Translate(Vector3.forward * hideSpeed * Time.deltaTime);
    }
}
