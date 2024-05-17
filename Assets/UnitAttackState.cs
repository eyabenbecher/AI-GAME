using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitAttackState : StateMachineBehaviour
{
    NavMeshAgent agent;
    AttackController attackController;

    public float stopAttackingDistance = 1.2f;


    public float attackRate = 2f;
    private float attackTimer;

    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        attackController = animator.GetComponent<AttackController>();
        attackController.SetAttackMaterial();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(attackController.targetToAttack !=null && animator.transform.GetComponent<UnitMovement>().isCommandedToMove == false)
        {

            LookAtTarget();
            // keep moving towards enemy
            agent.SetDestination(attackController.targetToAttack.position);

            if (attackTimer<=0)
            {
                Attack();

                attackTimer = 1f/attackRate;
            }
            else
            {
                attackTimer -= Time.deltaTime;
            }


            //should unit transision to attack state
            float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);
            if (distanceFromTarget > stopAttackingDistance || attackController.targetToAttack == null)
            {
                animator.SetBool("isAttacking", false); // Move to Attacking state
            }

        }
    }


    private void Attack()
    {
        var damageToInflict = attackController.unitDamage;

        // Actually Attack unit

        attackController.targetToAttack.GetComponent<Unit>().TakeDamage(damageToInflict);
    }


    private void LookAtTarget()
    {
        Vector3 direction = attackController.targetToAttack.position - agent.transform.position;
        agent.transform.rotation = Quaternion.LookRotation(direction);


        var yRotation = agent.transform.localEulerAngles.y;
        agent.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    
}
