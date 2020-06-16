using UnityEngine;

namespace enemies
{
    public class WaitPoint : MonoBehaviour , IWaitPoint
    {
        [SerializeField]
        private float _waitTime = 1;


        public float waitTime => _waitTime;
    }

    public interface IWaitPoint
    {
        float waitTime { get; }
    }
}