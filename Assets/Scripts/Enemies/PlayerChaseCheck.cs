﻿using System;
using Core;
using UnityEngine;

namespace enemies
{
    public class PlayerChaseCheck : MonoBehaviour
    {
        private CheckForDiverHit _checkHitDiver;
        private IEel eel;
        [SerializeField] private string deathMessage = "Diver was eaten by an eel!";
        private void Awake()
        {
            this._checkHitDiver = new CheckForDiverHit(transform, 0.125f);
            this.eel = GetComponent<IEel>();
        }

       

        private void Update()
        {
            if (WasDiverKilled())
            {
                eel.StopChasing();
            }
        }

        private bool WasDiverKilled()
        {
            if (!IsEelTargetingDiver()) return false;
            var hitDiver = _checkHitDiver.IsDiverInRadius();
            if (hitDiver)
            {
                GameManager.Instance.KillDiver(deathMessage);
            }
            return hitDiver;
        }

        private bool IsEelTargetingDiver()
        {
            return eel.CurrentTarget != null && eel.CurrentTarget.CompareTag("Player");
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !GameManager.Instance.IsPlayerHidden)
            {
               // eel.CurrentTarget = other.transform;
                eel.StartChasing(other.transform);
            }
            else if (other.CompareTag("Player") && eel.CurrentTarget == other.transform &&
                     GameManager.Instance.IsPlayerHidden)
            {
                eel.StopChasing();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player") && (eel.CurrentTarget == other.transform))
            {
                eel.StopChasing();
            }
        }
    }
}