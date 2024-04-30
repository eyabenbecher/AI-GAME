using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class fuire : ActionNode
{

    //private GameObject npc;
    //private GameObject[] hidingSpots;
    //private float hideTimer = 0.0f;
    //public float hideInterval = 3.0f;
    //public float minDistance = 1.0f;
    //private bool isHiding = false;

    //protected override void OnStart()
    //{
    //    npc = context.gameObject;
    //    hidingSpots = GameObject.FindGameObjectsWithTag("HidingSpot");
    //}

    //protected override void OnStop()
    //{
    //    isHiding = false;
    //}

    //protected override State OnUpdate()
    //{
    //    if (isHiding)
    //    {
    //        hideTimer += Time.deltaTime;
    //        if (hideTimer >= hideInterval)
    //        {
    //            return State.Success; // Successfully hidden
    //        }
    //        return State.Running;
    //    }
    //    else
    //    {
    //        if (FindHidingSpot())
    //        {
    //            isHiding = true;
    //            return State.Running;
    //        }
    //        else
    //        {
    //            return State.Failure; // No suitable hiding spot found
    //        }
    //    }
    //}

    //private bool FindHidingSpot()
    //{
    //    foreach (GameObject spot in hidingSpots)
    //    {
    //        float distance = Vector3.Distance(npc.transform.position, spot.transform.position);
    //        if (distance > minDistance)
    //        {
    //            npc.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(spot.transform.position);
    //            return true;
    //        }
    //    }
    //    return false; // No suitable hiding spot found
    //}


    private GameObject player;
    private GameObject guard;
    public guardProp guardProp;

    public float baseFleeSpeed = 20f;
    public float fleeRotationSpeed = 10f;
    public float fleeDistanceThreshold = 10f;
    public float dangerDistance = 5f;
    public float dangerFleeSpeedMultiplier = 2f;
    public float safeDistance = 20f;
    public float safeFleeSpeedMultiplier = 0.5f;

    protected override void OnStart()
    {
        player = GameObject.Find("Player");
        guard = context.gameObject;

        guardProp = guard.GetComponent<guardProp>();
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        float distanceToPlayer = Vector3.Distance(guard.transform.position, player.transform.position);
        float fleeSpeed = CalculateFleeSpeed(distanceToPlayer);

        RunAway(player.transform, fleeSpeed);

        if (distanceToPlayer > fleeDistanceThreshold)
        {
            return State.Success;
        }
        else
        {
            return State.Running;
        }
    }

    private float CalculateFleeSpeed(float distanceToPlayer)
    {
        float fleeSpeed = baseFleeSpeed;

        // Adjust flee speed based on distance to player
        if (distanceToPlayer < dangerDistance)
        {
            fleeSpeed *= dangerFleeSpeedMultiplier; // Flee faster if danger is nearby
        }
        else if (distanceToPlayer > safeDistance)
        {
            fleeSpeed *= safeFleeSpeedMultiplier; // Flee slower if the guard feels safe
        }

        return fleeSpeed;
    }

    public void RunAway(Transform player, float fleeSpeed)
    {
        Vector3 directionToPlayer = guard.transform.position - player.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        guard.transform.rotation = Quaternion.Slerp(guard.transform.rotation, targetRotation, Time.deltaTime * fleeRotationSpeed);

        guard.transform.Translate(Vector3.forward * fleeSpeed * Time.deltaTime);
    }
}