using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Example of destructible - damaged by raycast

public class Target : MonoBehaviour, IDamageable
{
   [SerializeField] private float health = 100f;

   public void TakeDamage(float damage)
   {
      health -= damage;
      if (health <= 0)
      {
         Destroy(gameObject);
      }

   }
}