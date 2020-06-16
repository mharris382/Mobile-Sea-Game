using Core;
using UnityEngine;

namespace enemies
{
    public class Jellyfish : MonoBehaviour
    {
        private const string DEATH_MESSAGE = "The diver got stung by a jellyfish!";
        [Range(-1,1)]
        public float safeZoneDot = 0;
        
        
        private void OnCollisionEnter2D(Collision2D other)
        {
        
            var hit = other.contacts[0];
            if (!hit.collider.CompareTag("Player"))
                return;
                
            var noraml = hit.normal;
            var up = transform.up;
            var dot = Vector2.Dot(noraml, up);
            
            if (dot > safeZoneDot)
                return;
            
            
            GameManager.Instance.KillDiver(DEATH_MESSAGE);

        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.Instance.KillDiver(DEATH_MESSAGE);
            }
        }
    }
}