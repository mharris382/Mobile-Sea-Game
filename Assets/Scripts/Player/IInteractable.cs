using UnityEngine;
namespace Player
{
    public interface IInteractable
    {
        
        Rigidbody2D rigidbody2D { get; }

        string name { get; }

    }


    public abstract class InteractableBase : MonoBehaviour, IInteractable
    {
        private Rigidbody2D _rb;
        public new Rigidbody2D rigidbody2D => _rb ?? (_rb = GetComponent<Rigidbody2D>());
    }
}