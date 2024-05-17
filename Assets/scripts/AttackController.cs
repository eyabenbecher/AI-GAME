using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class AttackController : MonoBehaviour
{
    public Transform targetToAttack;

    public Material idleStateMaterial;
    public Material followStateMaterial;
    public Material attackStateMaterial;
    public int unitDamage;

    public bool isPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if(isPlayer && other.CompareTag("Enemy") &&  targetToAttack == null)
        {
            targetToAttack = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isPlayer && other.CompareTag("Enemy") && targetToAttack != null)
        {
            targetToAttack = null;
        }
    }


    public void SetIdleMaterial()
    {
        GetComponent<Renderer>().material= idleStateMaterial;
    }
    public void SetFollowMaterial()
    {
        GetComponent<Renderer>().material = followStateMaterial;
    }
    public void SetAttackMaterial()
    {
        GetComponent<Renderer>().material = attackStateMaterial;
    }


    private void OnDrawGizmos()
    {
        // Follow Distance / Area
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,10f);


        // Attack Distance
        Gizmos.color= Color.red;
        Gizmos.DrawWireSphere(transform.position, 5f);


        // Stop Attack Distance
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 6f);


    }
}
