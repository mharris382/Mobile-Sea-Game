using UnityEngine;

namespace Player.Diver
{
    public class DiverJointHolderFactory : IFactory<Holder.JointHolderBase, Rigidbody2D>
    {
        private Rigidbody2D _diverMovement;
        private readonly Transform _attachPoint;

        public DiverJointHolderFactory(Rigidbody2D diverMovement, Transform attachPoint)
        {
            this._diverMovement = diverMovement;
            _attachPoint = attachPoint;
        }

        public Holder.JointHolderBase Create(Rigidbody2D holdable)
        {
            float distance = Vector2.Distance(_diverMovement.position, holdable.position);
            
            if (holdable.isKinematic || holdable.CompareTag("Hook"))
            {
                return new Holder.FixedJointHolder(holdable, _diverMovement, _attachPoint, distance/2f );
            }

            return new Holder.SpringJointHolder(holdable, _diverMovement, distance);
        }
    }
    
    public interface IFactory<out T,in T1>
    {
        T Create(T1 arg1);
    }
}