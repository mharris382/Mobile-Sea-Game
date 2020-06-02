using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using enemies;
using UnityEngine;
using UnityEngine.InputSystem;

namespace playerAbilities
{
    public class FishDisruptor : MonoBehaviour
    {
        public bool IsActive
        {
            get => GameManager.Instance.IsDisruptorActive;
            private set => GameManager.Instance.IsDisruptorActive = value;
        }
        public int maxDisruptionTime = 10;
        public int cooldownTime = 0;

        private int _countdown;

        

        public void StartDisruption()
        {
            if (_countdown > 0)
                return;

            IsActive = true;
            _countdown = maxDisruptionTime;
            StartCoroutine(DoCountdown());;
        }

        public void StopDisruption()
        {
            if (!IsActive)
                return;
            
            IsActive = false;
            StopCoroutine(DoCountdown());
            _countdown = cooldownTime;
            StartCoroutine(DoCountdown());
        }

        private IEnumerator DoCountdown()
        {
            yield return new WaitForSeconds(1);
            while (_countdown > 0)
            {
                _countdown--;
                yield return new WaitForSeconds(1);
            }
            if(IsActive) StopDisruption();
            yield return null;
        }

        public void OnDisruptorPressed(InputAction.CallbackContext context)
        {
            StartDisruption();
        }
    }
}
