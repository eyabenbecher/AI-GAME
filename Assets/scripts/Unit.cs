using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Unit : MonoBehaviour
{

    public float unitHealth;
    public float unitMaxHealth ;

    public HealthTracker healthTracker;

    void Start()
    {
        //UnitSelectionManager.Instance.allUnitsList.Add(gameObject);

        unitHealth = unitMaxHealth;

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        healthTracker.UpdateSliderValue(unitHealth, unitMaxHealth);

        if(unitHealth <= 0)
        {
            //Destruction or Dying animation / sound effect
            Destroy(gameObject);
        }
    }

    //private void OnDestroy()
    //{
    //    UnitSelectionManager.Instance.allUnitsList.Remove(gameObject);
    //}

    internal void TakeDamage(int damageToInflict)
    {
        unitHealth -= damageToInflict;
        UpdateHealthUI();
    }
    internal void Heal(float healAmount)
    {
        unitHealth = Mathf.Min(unitHealth + healAmount, unitMaxHealth);
        UpdateHealthUI();
    }

    internal float GetHealthPercentage()
    {
        return unitHealth / unitMaxHealth;
    }
}
