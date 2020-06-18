using UnityEngine;

namespace enemies
{
    public interface IEel
    {
        Transform CurrentTarget { get; }
        void StopChasing();
        void StartChasing(Transform transform);
    }
}