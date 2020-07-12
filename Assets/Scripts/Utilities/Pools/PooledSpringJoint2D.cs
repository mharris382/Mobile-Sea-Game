using UnityEngine;

namespace Utilities.Pools
{
    [RequireComponent(typeof(SpringJoint2D), typeof(Rigidbody2D))]
    public class PooledSpringJoint2D : PooledMonoBehaviour
    {
        private SpringJoint2D _springJoint;
        private Rigidbody2D _rigidbody2D;
        private void Awake()
        {
            _rigidbody2D = this.GetComponent<Rigidbody2D>();
            _springJoint = this.GetComponent<SpringJoint2D>();
        }

        protected override void OnDisable()
        {
            _springJoint.connectedBody = null;
            base.OnDisable();
        }
    }
}