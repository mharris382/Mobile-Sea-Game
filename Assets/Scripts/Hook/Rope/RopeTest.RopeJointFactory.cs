using System;
using Hook.Rope;
using UnityEngine;
using Zenject;
using RopeJoint = RopeTest.RopeJoint;

public partial class RopeTest
{
    public class RopeJointFactory : IFactory<RopeJoint, RopeJoint>, IFactory<IRopeJoint, IRopeJoint>
    {
        private readonly PooledRopeJoint _prefab;
        private readonly Transform _parent;
        private readonly Rigidbody2D _connected;

        public RopeJointFactory(PooledRopeJoint prefab,Transform parent, Rigidbody2D connected)
        {
            _prefab = prefab;
            _parent = parent;
            _connected = connected;
        }

        public RopeJoint Create(RopeJoint prevJoint)
        {
            if (prevJoint == null)
            {
                return CreateFirstJoint();
            }
                
            Vector2 spawnPosition = prevJoint.AttachedBody.transform.position ;
            spawnPosition += (Vector2.down * (_prefab.Distance / 2f));
                
                
            PooledRopeJoint newPooled = _prefab.Get<PooledRopeJoint>(spawnPosition , Quaternion.identity, _parent);
            prevJoint.ConnectedBody = newPooled.AttachedBody;

            var joint = new RopeJoint(newPooled, prevJoint, this);
            joint.ConnectedBody = _connected;
            // prevJoint.ConnectedBody = joint.AttachedBody;
            newPooled.RopeJoint = joint;
                
                
              
            return joint;
        }

        private RopeJoint CreateFirstJoint()
        {
            Vector2 spawnPosition = _parent.position;
            PooledRopeJoint newPooled = _prefab.Get<PooledRopeJoint>(spawnPosition, Quaternion.identity, _parent);
            newPooled.AttachedBody.isKinematic = true;
            var joint =  new RopeJoint(newPooled, null, this)
            {
                ConnectedBody = _connected
            };
            newPooled.RopeJoint = joint;
            return joint;
        }

        public IRopeJoint Create(IRopeJoint prevJoint)
        {
            if (prevJoint == null)
                return CreateFirstJoint();
            else if (prevJoint is RopeJoint)
                return Create(prevJoint as RopeJoint);
            
            throw new NotImplementedException();
        }
    }
}