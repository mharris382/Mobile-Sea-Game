using System;
using System.Collections;
using System.Linq;
using UnityEngine;
namespace Player
{
    public class Holder : MonoBehaviour
    {

        private IHoldable _heldObject;
        private IDisposable _holderJoint;

        public new Rigidbody2D rigidbody2D { get; private set; }

        public IHoldable HeldObject
        {
            get { return _heldObject; }
            set { _heldObject = value; }
        }


        private void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
        }


        public bool TryHoldObject(IHoldable objectToHold, JointHolderBase jointHolder)
        {
            
            ReleaseObject();
            

            //if object says no then don't attach
            if (!objectToHold.CanBePickedUpBy(this))
                return false;

            //if joint says no then don't attach
            if (!jointHolder.CanBeAttached())
                return false;
            
            _holderJoint = jointHolder;
            jointHolder.TargetPoint = transform.position;
            jointHolder.Attach();

            return true;

        }

        public void ReleaseObject()
        {
            if(_heldObject != null)
            {
                //TODO: notify object that it was released
            }
            if(_holderJoint != null)
            {
                _holderJoint.Dispose();
                _holderJoint = null;
            }
        }
        public abstract class JointHolderBase : IDisposable
        {
            protected readonly Rigidbody2D _heldBody;
            public bool isAttached { get; private set; } = false;
            public bool isDisposed { get; private set; } = false;

            public float maxDistance { get; }
            public abstract Vector2 TargetPoint { get; set; }


            public JointHolderBase(Rigidbody2D heldBody, float maxDistance)
            {
                this._heldBody = heldBody;
                this.maxDistance = maxDistance;
            }

            public bool CanBeAttached() => !isAttached;// Vector2.Distance(TargetPoint, m_heldBody.position) < maxDistance;
            public void Attach()
            {
                if (isAttached)
                    return;

                isAttached = true;
                _Attach();
            }
            protected abstract void _Attach();


            public abstract event Action OnJointBroke;
            public void Dispose()
            {
                isAttached = false;
                isDisposed = true;
                _Dispose();
            }
            protected abstract void _Dispose();
        }

        public class TargetJointHolder : JointHolderBase
        {
            private readonly Vector2 m_startTargetPoint;
            private readonly float maxLength;
            private readonly float m_frequency;
            private readonly float m_dampening;
            private TargetJoint2D m_targetJoint;

            public override event Action OnJointBroke;

            public TargetJointHolder(Rigidbody2D heldBody, Vector2 startTargetPoint, float maxLength) : this(heldBody, startTargetPoint, maxLength, 4f, 1) { }
            public TargetJointHolder(Rigidbody2D heldBody, Vector2 startTargetPoint, float maxLength, float frequency, float dampening) : base(heldBody, maxLength)
            {
                m_startTargetPoint = startTargetPoint;

                this.maxLength = maxLength;
                m_frequency = frequency;
                m_dampening = dampening;
            }
            protected override void _Attach()
            {
                m_targetJoint = _heldBody.gameObject.AddComponent<TargetJoint2D>();
                IsActive = true;
                m_targetJoint.target = m_startTargetPoint;
                m_targetJoint.autoConfigureTarget = false;
                m_targetJoint.frequency = m_frequency;
                m_targetJoint.dampingRatio = m_dampening;
            }
            public void Detach()
            {
                Destroy(m_targetJoint);
            }
            public override Vector2 TargetPoint
            {
                get => m_targetJoint.target;
                set => m_targetJoint.target = value;
            }
            public float Length => (m_targetJoint.target - _heldBody.position).magnitude;
            public bool IsActive { get; private set; }

            public void SetTargetPoint(Vector2 value)
            {
                var delta = _heldBody.position - value;
                delta = Vector2.ClampMagnitude(delta, maxLength);
                m_targetJoint.target += delta;
            }

            protected override void _Dispose()
            {
                if (IsActive == false) return;
                Detach();
                OnJointBroke?.Invoke();
            }

            
        }

        public class SpringJointHolder : JointHolderBase
        {
            private Rigidbody2D m_holderBody;
            private readonly float m_distance;
            private readonly float m_frequency;
            private readonly float m_dampening;
            private SpringJoint2D joint;
            private bool disposed = false;

            public SpringJointHolder(Rigidbody2D heldBody, Rigidbody2D holderBody, float distance) : this(heldBody, holderBody, distance, 2.5f, 1) { }
            public SpringJointHolder(Rigidbody2D heldBody, Rigidbody2D holderBody, float distance, float frequency, float dampening) : base(heldBody, distance)
            {
                disposed = true;
                m_holderBody = holderBody;
                m_distance = distance;
                m_frequency = frequency;
                m_dampening = dampening;
            }

            private IEnumerator CheckForBroken()
            {
                while (!disposed)
                {
                    if (joint.connectedBody != m_holderBody)
                    {
                        OnJointBroke?.Invoke();
                        _Dispose();
                    }
                    yield return null;
                }
            }
            protected override void _Attach()
            {
                this.joint = _heldBody.gameObject.AddComponent<SpringJoint2D>();
                joint.autoConfigureDistance = false;
                joint.connectedBody = m_holderBody;
                joint.distance = m_distance;
                joint.dampingRatio = m_dampening;
                joint.frequency = m_frequency;
                Core.GameManager.Instance.StartCoroutine(CheckForBroken());
            }
            public override Vector2 TargetPoint
            {
                get => m_holderBody.position;
                set => m_holderBody.MovePosition(value);
            }



            public override event Action OnJointBroke;

            protected override void _Dispose()
            {
                if (this.joint != null)
                    GameObject.Destroy(this.joint);
            }


        }

       public class FixedJointHolder : JointHolderBase
       {
           private readonly Rigidbody2D _holder;
           private readonly Transform _attachPoint;
           private readonly  Transform _detachedParent;
           private readonly bool _alwaysKinematic;
           private Collider2D[] _heldColliders;
           private Collider2D[] _holderColliders;

           public FixedJointHolder(Rigidbody2D heldBody, Rigidbody2D holder, Transform attachPoint, float maxDistance) : base(heldBody, maxDistance)
           {
               _holder = holder;
               _attachPoint = attachPoint;
               _detachedParent = heldBody.transform.parent;
               _alwaysKinematic = heldBody.isKinematic;
               _holderColliders = _holder.GetComponents<Collider2D>().Where(t => !t.isTrigger).ToArray();
               _heldColliders = _heldBody.GetComponents<Collider2D>().Where(t => !t.isTrigger).ToArray();
           }

           public override Vector2 TargetPoint
           {
               get => _attachPoint.position;
               set =>  _attachPoint.position = value;
           }

           protected override void _Attach()
           {
               _heldBody.MovePosition(_attachPoint.position);
               _heldBody.transform.parent = _attachPoint;

               
               SetCollisionsIgnored(true);
               
           }

           private void SetCollisionsIgnored(bool ignore)
           {
               foreach (var holderCollider in _holderColliders)
               {
                   foreach (var heldCollider in _heldColliders)
                   {
                       Physics2D.IgnoreCollision(holderCollider, heldCollider, ignore);
                   }
               }
           }

           public override event Action OnJointBroke;
           
           protected override void _Dispose()
           {
               _heldBody.isKinematic = _alwaysKinematic;
               _heldBody.transform.parent = _detachedParent;
               OnJointBroke?.Invoke();
               SetCollisionsIgnored(false);
           }
       }
    }



}