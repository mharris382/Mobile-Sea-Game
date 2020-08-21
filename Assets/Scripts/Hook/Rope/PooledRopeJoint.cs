using System;
using Sirenix.Utilities;
using UnityEngine;
using Utilities.Pools;

namespace Hook.Rope
{
    [RequireComponent(typeof(DistanceJoint2D))]
    public class PooledRopeJoint : PooledMonoBehaviour
    {
        private DistanceJoint2D __joint;
        private DistanceJoint2D Joint => __joint ? __joint : (__joint = GetComponent<DistanceJoint2D>());
        private AnchoredJoint2D[] _joints;
        private Rigidbody2D _rb;

        public Vector2 Anchor
        {
            get => Joint.anchor;
            set
            {
                foreach (var joint in _joints)
                {
                    joint.anchor = value;
                }
            }
        }
        public Vector2 ConnectedAnchor
        {
            get => Joint.connectedAnchor;
            set
            {
                
                foreach (var joint in _joints)
                {
                    joint.connectedAnchor = value;
                }
            }
        }
        public Rigidbody2D ConnectedBody
        {
            get => Joint.connectedBody;
            set
            {
                foreach (var joint in _joints)
                {
                    joint.connectedBody = value;
                }
            }
        }
        public Rigidbody2D AttachedBody => Joint.attachedRigidbody;

        public float Distance
        {
            get => Joint.GetDistance();
            set => _joints.ForEach(t => t.SetDistance(value));
        }

        public RopeTest.RopeJoint RopeJoint { get; set; }

        private void Awake()
        {
            this._rb = GetComponent<Rigidbody2D>();
            _joints = GetComponents<AnchoredJoint2D>();
            __joint = GetComponent<DistanceJoint2D>();
            Debug.Assert(Joint.GetDistance()!=-1, "Distance Joint is not the first joint in the list");
        }

        // internal void SetDistance(float targetDistance)
        // {
        //     float dist = 0;
        //     SetDistance(targetDistance, ref dist);
        // }
        //
        // internal void SetDistance(float targetDistance, ref float currentDistance)
        // {
        //     throw new System.NotImplementedException();
        // }
    }


    public static class JointExtensions
    {
        public static float GetDistance(this AnchoredJoint2D joint)
        {
            switch (joint)
            {
                case DistanceJoint2D distanceJoint2D:
                    return distanceJoint2D.distance;
                    break;
                
                case SpringJoint2D springJoint2D:
                    return springJoint2D.distance;
                    break;
                default:
                    return -1;
            }
        }

        public static void SetDistance(this AnchoredJoint2D joint, float value)
        {
            switch (joint)
            {
                case DistanceJoint2D distanceJoint2D:
                    distanceJoint2D.distance = value;
                    break;
                case SpringJoint2D springJoint2D:
                    springJoint2D.distance = value;
                    break;
                
            }
        }
    }
}