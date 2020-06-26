using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Player
{
    public class InteractionTrigger : MonoBehaviour
    {
        private List<Collider2D> _inRangeInteractions = new List<Collider2D>();

        private static Dictionary<Collider2D, IInteractable[]> interactableCache =
            new Dictionary<Collider2D, IInteractable[]>();


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

        public List<T> GetInRangeInteractables<T>() where T : class, IInteractable
        {
            return _inRangeInteractions.SelectMany(item => interactableCache[item]).Select(t => t as T)
                .Where(t => t != null).ToList();
        }


        public IEnumerable<IInteractable> GetInRangeInteractables(bool sortByDistance = false)
        {
            var res = (from col in _inRangeInteractions
                    where (interactableCache[col].Length > 0)
                    select interactableCache[col])
                .SelectMany(t => t);
            if (sortByDistance)
                return res.OrderBy(t => (t.rigidbody2D.position - (Vector2) transform.position).sqrMagnitude);
            return res;
        }

        public IEnumerable<IInteractable> GetInRangeInteractables(Vector2 relativeTo)
        {
            var res = (from col in _inRangeInteractions
                    where (interactableCache[col].Length > 0)
                    select interactableCache[col])
                .SelectMany(t => t);

            return res.OrderBy(t => (t.rigidbody2D.position - relativeTo).sqrMagnitude);
        }

        static class ColliderCache<T> where T : IInteractable
        {
            internal static Dictionary<Collider2D, T> interactableCache = new Dictionary<Collider2D, T>();
        }

        protected internal bool IsColliderInRange(Collider2D collider2D)
        {
            return interactableCache.ContainsKey(collider2D);
        }


        private class InteractableObservable : IObservable<IInteractable>
        {
            private InteractionTrigger _trigger;

            private IObserver<IInteractable[]> _interactableListener;

            public IDisposable Subscribe(IObserver<IInteractable> observer)
            {
                throw new NotImplementedException();
            }


            // IEnumerator Tick()
            // {
            //     while (true)
            //     {
            //         
            //         yield return new WaitForEndOfFrame();
            //         
            //     }
            // }
        }
    }
}