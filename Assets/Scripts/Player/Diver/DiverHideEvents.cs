using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.Events;

namespace Player.Diver
{
    public class DiverHideEvents : MonoBehaviour
    {
        public UnityEvent OnDiverHidden;
        public UnityEvent OnDiverUnhidden;
        public bool onlyFireWhenEnabled = true;

        private void Awake()
        {
            GameManager.OnPlayerHiddenChanged += isHidden =>
            {
                if (!enabled && onlyFireWhenEnabled)
                    return;

                if (isHidden)
                {
                    OnDiverHidden?.Invoke();
                }
                else
                {
                    OnDiverUnhidden?.Invoke();
                }
            };
        }

        private void Start()
        {
            OnDiverUnhidden?.Invoke();
        }
    }

}