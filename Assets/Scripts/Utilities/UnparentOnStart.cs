using UnityEngine;

namespace Utilities
{
    public class UnparentOnStart : MonoBehaviour
    {
        private void Start()
        {
            transform.SetParent(null);
        }
    }
}