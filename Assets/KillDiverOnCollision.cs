using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class KillDiverOnCollision : MonoBehaviour
{

   public string deathMessage = "Diver got pricked by a sea urchin!";
   
   private void OnCollisionEnter2D(Collision2D other)
   {
      if (other.collider.transform.CompareTag("Player"))
      {
         GameManager.Instance.KillDiver(other.rigidbody.gameObject, deathMessage);
      }
   }
}
