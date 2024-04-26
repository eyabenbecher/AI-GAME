using UnityEngine;
using TheKiwiCoder;
using UnityEngine.AI;

[System.Serializable]
public class Chaser1 : ActionNode
{
    private GameObject player;
    private GameObject guard;
    public guardProp guardProp;

    public float vitesse_poursuite;
    public float vitesseRot_poursuite;
    public float precision_poursuite;

    protected override void OnStart()
    {
        //Debug.Log("Chasing player...");
        player = GameObject.Find("Player");
        guard = context.gameObject;

        guardProp = guard.GetComponent<guardProp>();
        vitesse_poursuite = 20f;
        vitesseRot_poursuite = 10f;
        precision_poursuite = 10f;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        Poursuivre(player.transform);
        //agent.SetDestination(player.transform.position);

        if (Vector3.Distance(guard.transform.position, player.transform.position) < 1.0f)
        {
            return State.Success;
        }
        else
        {
            blackboard.moveToPosition = player.transform.position;
            return State.Running;
        }
    }
    public void Poursuivre(Transform Player)
    {
        guard.GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = true;
        guard.GetComponent<UnityEngine.AI.NavMeshAgent>().ResetPath();
        Vector3 direction = Player.position - guard.transform.position;
        guard.transform.rotation = Quaternion.Slerp(guard.transform.rotation, Quaternion.
         LookRotation(direction), Time.deltaTime * vitesseRot_poursuite);

        if (direction.magnitude > precision_poursuite)
        {
            guard.transform.Translate(0, 0, Time.deltaTime * vitesse_poursuite);
            //ici chisir le bon emplacement sur votre map !
        }

    }
}
