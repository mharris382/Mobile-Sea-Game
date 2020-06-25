using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Player
{

    public class InteractionTrigger : MonoBehaviour
    {
        private List<Collider2D> _inRangeInteractions = new List<Collider2D>();

        static Dictionary<Collider2D, IInteractable[]> interactableCache = new Dictionary<Collider2D, IInteractable[]>();

        private void OnTriggerEnter2D(Collider2D collision)
        {
            IInteractable[] interactables;
            if (interactableCache.TryGetValue(collision, out interactables) == false)
            {
                interactableCache.Add(collision, collision.GetComponents<IInteractable>());
                _inRangeInteractions.Add(collision);
                return;
            }
            if (interactables != null && interactables.Length > 0)
            {
                _inRangeInteractions.Add(collision);
            }
        }


        private void OnTriggerExit2D(Collider2D collision)
        {
            IInteractable[] interactables;
            if (interactableCache.TryGetValue(collision, out interactables) == false)
            {
                interactableCache.Add(collision, collision.GetComponents<IInteractable>());
            }
            if (interactables != null && interactables.Length > 0)
            {
                _inRangeInteractions.Remove(collision);
            }
        }

        public List<T> GetInRangeInteractables<T> () where T : class, IInteractable
        {
           return _inRangeInteractions.SelectMany(item => interactableCache[item]).Select(t => t as T).Where(t => t != null).ToList();
           
        }

        static class ColliderCache<T> where T : IInteractable
        {
            internal static Dictionary<Collider2D, T> interactableCache = new Dictionary<Collider2D, T>();
        }
    }

}