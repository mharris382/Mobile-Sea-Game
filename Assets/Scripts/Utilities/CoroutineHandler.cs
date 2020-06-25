using System;
using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class CoroutineHandler : MonoBehaviour
    {
        private static CoroutineHandler _instance;

        public static CoroutineHandler instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("StateMachineManager");
                    _instance = go.AddComponent<CoroutineHandler>();
                }

                return _instance;
            }
        }

        private void Awake()
        {
            _instance = this;
        }

        public void Invoke(Action action, float time)
        {
            StartCoroutine(CallInvoke(action, time));
        }

        private IEnumerator CallInvoke(Action action, float time)
        {
            yield return new WaitForSeconds(time);
            action?.Invoke();
        }
    }
}