using UnityEngine;
using TheKiwiCoder;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;

[System.Serializable]
public class Attack : ActionNode
{
    public string playerTag = "Player";
    public float damageAmount = 10f;

    private NavMeshAgent agent;
    private GameObject player;

    protected override void OnStart()
    {
        agent = context.gameObject.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found on NPC.");
            return;
        }

        // Find the player dynamically by tag
        player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null)
        {
            Debug.LogError("Player GameObject not found with tag: " + playerTag);
            return;
        }

        agent.SetDestination(player.transform.position);
    }

    protected override void OnStop()
    {
        //if (agent != null)
        //{
        //    Debug.Log("REsetingPAth");
        //    agent.ResetPath();
        //}
    }

    protected override State OnUpdate()
    {
        if (player == null)
        {
            Debug.LogError("Player GameObject not found with tag: " + playerTag);
            return State.Failure;
        }

        // Check if NPC has reached the player
        while (Vector3.Distance(context.transform.position, player.transform.position) <= 10)
        {
            Debug.Log("DamagingPlayer");
            // Deal damage to the player
            DealDamageToPlayer();
            return State.Success;
        }

        // Continue moving towards the player
        agent.SetDestination(player.transform.position);
        return State.Running;
    }

    private void DealDamageToPlayer()
    {
        // Example: Deal damage to the player
        Unit unit = player.GetComponent<Unit>();
        if (unit != null)
        {
            unit.TakeDamage((int)damageAmount);
        }
        else
        {
            Debug.LogError("Player Unit component not found.");
        }
    }
}
