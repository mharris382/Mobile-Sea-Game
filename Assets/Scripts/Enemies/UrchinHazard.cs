using System;
using Core;
using UnityEngine;

namespace enemies
{
    public class UrchinHazard : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.Instance.KillDiver("The diver got pricked by a sea urchin!");
            }
        }
    }
}
