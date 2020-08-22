using System;
using Hook.Rope;
using UnityEngine;
using Zenject;

public partial class RopeTest
{
    public sealed class RopeJoint : IRopeJoint
    {
        private float _maxJointDistance = 0.26f;

        private RopeJoint _prevJoint;
        private RopeJoint _nextJoint;
        
        // private readonly DistanceJoint2D _joint;
        // private readonly SpringJoint2D _springJoint2D;
        private readonly PooledRopeJoint _ropeJoint;
        private readonly IFactory<RopeJoint, RopeJoint> _factory;

        public RopeJoint NextJoint
        {
            get => _nextJoint;
            set => _nextJoint = value;
        }

        public RopeJoint PrevJoint
        {
            get => _prevJoint;
            set => _prevJoint = value;
        }
        IRopeJoint IRopeJoint.NextJoint => _nextJoint;

        IRopeJoint IRopeJoint.PrevJoint => _prevJoint;

        public Vector2 ConnectedAnchor
        {
            get => _ropeJoint.ConnectedAnchor;
            set => _ropeJoint.ConnectedAnchor = value;
        }

        public Vector2 Anchor
        {
            get => _ropeJoint.Anchor;
            set => _ropeJoint.Anchor = value;
        }
        
        public Rigidbody2D ConnectedBody
        {
            get => _ropeJoint.ConnectedBody;
            set => _ropeJoint.ConnectedBody = value;
        }

        public Rigidbody2D AttachedBody => _ropeJoint.AttachedBody;

        #region [Depricated Constructors]

        // [Obsolete("Use Factory Constructor")]
        // public RopeJoint(DistanceJoint2D joint, RopeJoint prevJoint)
        // {
        //     _joint = joint;
        //     _springJoint2D = _joint.GetComponent<SpringJoint2D>();
        //     _prevJoint = prevJoint;
        // }
        //
        // [Obsolete("Use PooledRopeJoint Constructor")]
        // public RopeJoint(DistanceJoint2D joint, RopeJoint prevJoint, IFactory<RopeJoint, RopeJoint> jointFactory)
        // {
        //     this._factory = jointFactory; 
        //     _joint = joint;
        //     _springJoint2D = _joint.GetComponent<SpringJoint2D>();
        //     _prevJoint = prevJoint;
        // }
        //

        #endregion
        
        public RopeJoint(PooledRopeJoint joint, RopeJoint prevJoint, IFactory<RopeJoint, RopeJoint> jointFactory)
        {
            Debug.Assert(joint!=null,"null joint");
            Debug.Assert(jointFactory != null, "Null Factory");
            this._factory = jointFactory;
            this._ropeJoint = joint;           
            _prevJoint = prevJoint;
        }


        public float GetTotalDistance()
        {
            float dist = 0;
            return GetTotalDistance(ref dist);
        }

        public void SetDistance(float target)
        {
            if (target <= 0.1f) return;
            float dist = 0;
            SetDistance(target, ref dist);
        }

        private void SetDistance(float targetDistance, ref float currentDistance)
        {
            //target distance was reached all remaining joints are not necessary
            if (currentDistance >= targetDistance)
            {
                _nextJoint?.SetDistance(targetDistance, ref currentDistance);
                
                DestroySelf();
                return;
            }

            float distance = targetDistance - currentDistance;

            if (distance > _maxJointDistance)
            {
                //new joint is needed so max out this joint and if we don't have a next joint add one 
                distance = _maxJointDistance;
                if (_nextJoint == null)
                {
                    try
                    {
                        _nextJoint = _factory.Create(this);
                    }
                    catch (Exception e)
                    {
                        if (_factory == null)
                            Debug.LogError("Null Joint Factory in RopeJoint", _ropeJoint);
                        _nextJoint = new RopeJoint(_ropeJoint.Get<PooledRopeJoint>(_ropeJoint.AttachedBody.position, Quaternion.identity, _ropeJoint.transform.parent),this, _factory);
                    }
                    ConnectedBody = _nextJoint._ropeJoint.AttachedBody;
                    
                }
            }

            
            _ropeJoint.Distance = distance;
            currentDistance += distance;
            _nextJoint?.SetDistance(targetDistance, ref currentDistance);
        }


        private float GetTotalDistance(ref float currentDistance)
        {
            currentDistance += _ropeJoint.Distance;
            return _nextJoint?.GetTotalDistance(ref currentDistance) ?? currentDistance;
        }

        private void DestroySelf()
        {
            if ( _prevJoint != null )
            {
                _prevJoint.ConnectedBody = ConnectedBody;
                _prevJoint.NextJoint = null;
                ConnectedBody = null;
            }
            
            // Destroy(_joint.attachedRigidbody.gameObject);
            _ropeJoint.gameObject.SetActive(false);
        }
    }
}