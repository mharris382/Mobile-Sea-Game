using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

public class RopeTest : MonoBehaviour
{
    private float _minDistance;
    
    private RopeJoint _head;
    private FirstRopeJoint _first;
    
    public Rigidbody2D connected;
    public RopeJoint EndJoint { get; }
    public IEnumerable<RopeJoint> AllJoints => _first;
    public IEnumerable<Vector2> JointPositions => _first.Select(t => t.AttachedBody.position);
    
    [InlineButton("Plus1", "+"), InlineButton("Minus1", "-")]
    [ShowInInspector, HideInEditorMode, MinValue("_minDistance")]
    public float Distance
    {
        get => GetDistance();
        set => SetDistance(value);
    }

    void Plus1() => Distance += 0.1f;

    void Minus1() => Distance -= 0.1f;

   private ReactiveProperty<float> _distance = new ReactiveProperty<float>();

   public UniRx.IObservable<float> GetDistanceStream() => _distance;
    
    private void Awake()
    {
        var joints = GetComponentsInChildren<DistanceJoint2D>();
        if (joints == null)
        {
            Debug.LogError("No child Distance Joints", this);
            return;
        }

        _first = new FirstRopeJoint(joints[0]);
        _head = _first;
        for (int i = 1; i < joints.Length; i++)
        {
            _head.NextJoint = new RopeJoint(joints[i], _head);
            _head = _head.NextJoint;
        }

        _minDistance = _first.GetTotalDistance();
        _head.ConnectedBody = connected;
        _distance.Value = _minDistance;
    }


    public void SetDistance(float dist)
    {
        if (_first == null) return;
        dist = Mathf.Max(dist, _minDistance);
        _first.SetDistance(dist);
        _distance.Value = dist;
    }

    public float GetDistance()
    {
        if (_first == null) return -1;
        return _first.GetTotalDistance();
    }


    private void LateUpdate()
    {
        foreach (var joint in _first)
        {
            joint.Anchor = Vector2.zero;
            joint.ConnectedAnchor = Vector2.zero;
            
        }
    }

    public class FirstRopeJoint : RopeJoint , IEnumerable<RopeJoint>
    {
        public FirstRopeJoint(DistanceJoint2D joint) : base(joint, null)
        {
        }

        public float GetTotalDistance()
        {
            float curr = 0;
            return base.GetTotalDistance(ref curr);
        }

        public void SetDistance(float target)
        {
            float curr = 0;
            base.SetDistance(target, ref curr);
        }

        public IEnumerator<RopeJoint> GetEnumerator()
        {
            RopeJoint curr = this;
            while (curr != null)
            {
                yield return curr;
                curr = curr.NextJoint;
            }
        }
        public IEnumerable<RopeJoint> GetEnumerable()
        {
            RopeJoint curr = this;
            while (curr != null)
            {
                yield return curr;
                curr = curr.NextJoint;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        
        
        
    }
    public class RopeJoint
    {
        private float _maxDistance = 0.26f;

        private DistanceJoint2D _joint;
        private SpringJoint2D _springJoint2D;
        private RopeJoint _prevJoint;
        private RopeJoint _nextJoint;

        public RopeJoint NextJoint
        {
            get => _nextJoint;
            set => _nextJoint = value;
        }

        public Vector2 ConnectedAnchor
        {
            get => _joint.connectedAnchor;
            set
            {
                
                _joint.connectedAnchor = value;
                if (_springJoint2D != null) _springJoint2D.connectedAnchor = value;
            }
        }

        public Vector2 Anchor
        {
            get => _joint.anchor;
            set
            {
                _joint.anchor = value;
                if (_springJoint2D != null) _springJoint2D.anchor = value;
            }
        }
        public RopeJoint(DistanceJoint2D joint, RopeJoint prevJoint)
        {
            _joint = joint;
            _springJoint2D = _joint.GetComponent<SpringJoint2D>();
            _prevJoint = prevJoint;
        }

        public Rigidbody2D ConnectedBody
        {
            get => _joint.connectedBody;
            set
            {
                _joint.connectedBody = value;
                if (_springJoint2D != null) _springJoint2D.connectedBody = value;
            }
        }

        public Rigidbody2D AttachedBody => _joint.attachedRigidbody;

        public bool AllowDestruction { get; set; }
        protected void SetDistance(float targetDistance, ref float currentDistance)
        {
            //target distance was reached all remaining joints are not necessary
            if (currentDistance >= targetDistance)
            {
                _nextJoint?.SetDistance(targetDistance, ref currentDistance);
                
                 DestroySelf();
                return;
            }

            float distance = targetDistance - currentDistance;

            if (distance > _maxDistance)
            {
                //new joint is needed so max out this joint and if we don't have a next joint add one 
                distance = _maxDistance;
                if (_nextJoint == null)
                {
                    var transform1 = _joint.transform;
                    var newJoint = GameObject.Instantiate(_joint, _joint.attachedRigidbody.position, transform1.rotation, transform1.parent);
                    _nextJoint = new RopeJoint(newJoint, this);
                    ConnectedBody = _nextJoint._joint.attachedRigidbody;
                    
                }
            }

            
            _joint.distance = distance;
            currentDistance += distance;
            _nextJoint?.SetDistance(targetDistance, ref currentDistance);
        }

       

        protected float GetTotalDistance(ref float currentDistance)
        {
            currentDistance += _joint.distance;
            return _nextJoint?.GetTotalDistance(ref currentDistance) ?? currentDistance;
        }

        private void DestroySelf()
        {
            if (ConnectedBody != null && _prevJoint != null && _prevJoint.ConnectedBody == _joint.attachedRigidbody)
            {
                _prevJoint.ConnectedBody = ConnectedBody;
                ConnectedBody = null;
            }
            if (_prevJoint != null) _prevJoint._nextJoint = null;
            Destroy(_joint.attachedRigidbody.gameObject);
        }
    }
}