using UnityEngine;

namespace enemies
{
    public class Snail : MonoBehaviour
    {
        private void OnTriggerStay2D(Collider2D other)
        {
            var flattener = other.GetComponent<CoverFlattener>();
            if (flattener != null)
            {
                flattener.Flatten();
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            var flattener = other.GetComponent<CoverFlattener>();
            if (flattener != null)
            {
                flattener.Straighten();
            }
        }
    }
}