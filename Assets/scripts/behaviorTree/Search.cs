using UnityEngine;
using TheKiwiCoder;
using UnityEngine.AI;

[System.Serializable]
public class Search : ActionNode
{
    private GameObject guard;
    private NavMeshAgent agent;
    private GameObject player;

    protected override void OnStart()
    {
        //Debug.Log("Searching last seen position...");
        guard = context.gameObject;
        agent = guard.GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (Vector3.Distance(guard.transform.position, blackboard.moveToPosition) < 0.5f)
        {
            return State.Running;
        }
        else
        {
            agent.SetDestination(blackboard.moveToPosition);
            return State.Failure;
        }
    }
}
