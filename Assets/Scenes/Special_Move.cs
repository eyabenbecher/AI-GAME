using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class Special_Move : ActionNode
{
    private GameObject npc;
    private GameObject[] hidingSpots;
    private float hideTimer = 0.0f;
    public float hideInterval = 3.0f;
    public float minDistance = 1.0f;
    private bool isHiding = false;

    protected override void OnStart()
    {
        npc = context.gameObject;
        hidingSpots = GameObject.FindGameObjectsWithTag("HidingSpot");
    }

    protected override void OnStop()
    {
        isHiding = false;
    }

    protected override State OnUpdate()
    {
        if (isHiding)
        {
            hideTimer += Time.deltaTime;
            if (hideTimer >= hideInterval)
            {
                return State.Success; // Successfully hidden
            }
            return State.Running;
        }
        else
        {
            if (FindHidingSpot())
            {
                isHiding = true;
                return State.Running;
            }
            else
            {
                return State.Failure; // No suitable hiding spot found
            }
        }
    }

    private bool FindHidingSpot()
    {
        foreach (GameObject spot in hidingSpots)
        {
            float distance = Vector3.Distance(npc.transform.position, spot.transform.position);
            if (distance > minDistance)
            {
                npc.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(spot.transform.position);
                return true;
            }
        }
        return false; // No suitable hiding spot found
    }
}




