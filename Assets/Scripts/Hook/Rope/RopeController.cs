using System;
using System.Collections.Generic;
using UniRx;
using Zenject;

namespace Hook.Rope
{
    [Serializable]
    public class RopeConfig
    {
        public float minDistance = 1;
        public float maxDistance = 50;
    }
    
    public class RopeController
    {
        private ReactiveProperty<float> _distance;
        private PooledRopeJoint _prefab;
        private RopeConfig _config;
        private LinkedList<PooledRopeJoint> _joints;
        
        internal RopeController(PooledRopeJoint pooledRopeJointPrefab, RopeConfig config)
        {
            this._prefab = pooledRopeJointPrefab;
            this._config = config;
            _distance = new ReactiveProperty<float>(config.minDistance);
            _joints = new LinkedList<PooledRopeJoint>();
        }

      
        
        internal class LinkedJoint
        {
            private PooledRopeJoint body;
            private LinkedJoint _previousJoint;
            private LinkedJoint _nextJoint;
            private IFactory<LinkedJoint, LinkedJoint> _factory;

            public LinkedJoint(PooledRopeJoint body, LinkedJoint nextJoint, IFactory<LinkedJoint, LinkedJoint> factory)
            {
                this.body = body;
                _nextJoint = nextJoint;
                _factory = factory;
            }

            protected void SetDistance(float targetDistance, ref float currentDistance)
            {
                if (currentDistance >= targetDistance)
                {
                    
                }
            }


            private LinkedJoint CreateNewJoint()
            {
                return _factory.Create(this);
            }
            
            public class LinkedJointFactory : IFactory<LinkedJoint, LinkedJoint>
            {
                private PooledRopeJoint _prefab;

                public LinkedJointFactory(PooledRopeJoint prefab)
                {
                    _prefab = prefab;
                }

                public LinkedJoint Create(LinkedJoint prevJoint)
                {
                    var body = _prefab.Get<PooledRopeJoint>();
                    
                    throw new NotImplementedException();
                }
            }
        }

       
    }
}