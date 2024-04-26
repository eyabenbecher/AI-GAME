using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class CanSee : ActionNode
{
    public GameObject player;
    public GameObject guard;
    public guardProp guardProp;

    public float fovDistance;
    public float fovAngle;

    protected override void OnStart()
    {
        //Debug.Log("Checking if I can see the player...");
        player = GameObject.Find("Player");
        guard = context.gameObject;

        guardProp = guard.GetComponent<guardProp>();
        fovDistance = guardProp.fovDistance;
        fovAngle = guardProp.fovAngle;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        DrawFOV();
        if (ICanSee(player.transform, guard.transform))
        {
            return State.Success;
        }
        else
        {
            return State.Failure;
        }
    }
    private bool ICanSee(Transform player, Transform objectTransform)
    {
        Vector3 direction = player.position - objectTransform.position;
        float angle = Vector3.Angle(direction, objectTransform.forward);

        if (angle < fovAngle / 2f)
        {
            RaycastHit hit;

            if (Physics.Raycast(objectTransform.position, direction, out hit, fovDistance))
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    return true;
                }
            }
        }

        return false;
    }
    void DrawFOV()
    {
        Vector3 origin = context.transform.position;
        float halfFOV = fovAngle / 2.0f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * context.transform.forward;
        Vector3 rightRayDirection = rightRayRotation * context.transform.forward;

        Debug.DrawRay(origin, leftRayDirection * fovDistance, Color.green);
        Debug.DrawRay(origin, rightRayDirection * fovDistance, Color.green);

        Debug.DrawLine(origin, origin + leftRayDirection * fovDistance, Color.green);
        Debug.DrawLine(origin, origin + rightRayDirection * fovDistance, Color.green);


        float currentAngle = -halfFOV;
        float step = fovAngle / 10.0f;
        for (int i = 0; i < 10; i++)
        {
            Quaternion rotation = Quaternion.AngleAxis(currentAngle, Vector3.up);
            Vector3 direction = rotation * context.transform.forward;
            Debug.DrawRay(origin, direction * fovDistance, Color.green);
            currentAngle += step;
        }
    }
}
