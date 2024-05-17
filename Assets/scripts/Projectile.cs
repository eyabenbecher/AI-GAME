using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damageAmount = 10f; // The amount of damage the projectile deals
    public float destroyDelay = 0f; // Delay before the projectile is destroyed (if needed)

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has the tag "Player" or "Unit"
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Unit"))
        {
            // Get the Unit component from the collided object
            Unit unit = collision.gameObject.GetComponent<Unit>();

            if (unit != null)
            {
                // Apply damage to the unit
                unit.TakeDamage((int)damageAmount);
            }

            // Destroy the projectile
            Destroy(gameObject, destroyDelay);
        }
        else
        {
            // Destroy the projectile on any collision
            Destroy(gameObject, 5);
        }
    }
}
