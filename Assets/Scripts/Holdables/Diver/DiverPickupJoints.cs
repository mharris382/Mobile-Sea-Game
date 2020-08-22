using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

namespace Holdables.Diver
{
    [TypeInfoBox("Responsible for handling the physics involved with picking up objects")]
    public class DiverPickupJoints : MonoBehaviour
    {
       
        [SerializeField] 
        private JointHolder.Config jointsConfig;
        
        
        private Holder _holder;
        private Rigidbody2D _rb;
        private JointHolder _jointHolder;
        
        
        [Inject]
        void Inject(Holder diverHolder, Rigidbody2D rb)
        {
            this._holder = diverHolder;
            this._rb = rb;
        }
        
        
        
        private void Start()
        {
            _jointHolder = new JointHolder(_rb, jointsConfig);
            _holder.OnPickupAsObservable().Subscribe(AttachPickupJoint);
            _holder.OnReleasedAsObservable().Subscribe(DetachPickupJoint);
        }

        private void AttachPickupJoint(IHoldable pickedUp)
        {
            _jointHolder.Attach(pickedUp.rigidbody2D);
        }

        private void DetachPickupJoint(IHoldable released)
        {
            _jointHolder.Dispose();
        }

       
        
        public class JointHolder : IDisposable
        {
            [System.Serializable]
            public class Config
            {
                [AssetsOnly, Required] public SpringJoint2D springinJointTemplate;
                [AssetsOnly, Required] public DistanceJoint2D distanceJointTemplate;


                public AnchoredJoint2D[] CreateJoints(Rigidbody2D attached, Rigidbody2D connected)
                {
                    
                    var springinJoint = attached.gameObject.AddComponent<SpringJoint2D>();
                    var distanceJoint = attached.gameObject.AddComponent<DistanceJoint2D>();


                    springinJoint.connectedBody = connected;
                    distanceJoint.connectedBody = connected;

                    springinJoint.enableCollision = springinJointTemplate.enableCollision;
                    distanceJoint.enableCollision = distanceJointTemplate.enableCollision;
                    
                    springinJoint.distance = springinJointTemplate.distance;
                    distanceJoint.distance = distanceJointTemplate.distance;

                    springinJoint.autoConfigureDistance = false;
                    distanceJoint.autoConfigureDistance = false;
                    springinJoint.autoConfigureConnectedAnchor = springinJointTemplate.autoConfigureConnectedAnchor;
                    distanceJoint.autoConfigureConnectedAnchor = distanceJointTemplate.autoConfigureConnectedAnchor;
                    
                    distanceJoint.maxDistanceOnly = distanceJointTemplate.maxDistanceOnly;
                   
                    springinJoint.frequency = springinJointTemplate.frequency;
                    springinJoint.dampingRatio = springinJointTemplate.dampingRatio;
                    
                    return new AnchoredJoint2D[]
                    {
                        springinJoint,
                        distanceJoint
                    };
                }
            }
            
            private Rigidbody2D _holderBody;
            private AnchoredJoint2D[] _joints;
            private IDisposable _disposable;
            private Config _config;
            
            static Dictionary<Rigidbody2D, IDisposable> activeHeldObjects = new Dictionary<Rigidbody2D, IDisposable>();


            public JointHolder(Rigidbody2D holderBody,  Config config)
            {
                
                _holderBody = holderBody;
                _config = config;
            }

            public void Attach(Rigidbody2D heldBody)
            {
                if (activeHeldObjects.ContainsKey(heldBody))
                {
                    return;
                }
                Debug.Log("Creating Joints");
                activeHeldObjects.Add(heldBody, this);
                
                _joints = _config.CreateJoints(_holderBody, heldBody);
                
                
                ResetConnectedAnchors();
                _disposable = new CompositeDisposable(UniRx.
                        Disposable.Create(() => activeHeldObjects.Remove(heldBody)),
                        Observable.EveryUpdate().Subscribe(_ => ResetConnectedAnchors())
                    );
            }

            public void ResetConnectedAnchors()
            {
                foreach (var joint in _joints)
                {
                    joint.connectedAnchor = Vector2.zero;
                }
            }

            public void Dispose()
            {
                _disposable?.Dispose();
                Debug.Log("Destroying Joints");
                foreach (var joint in _joints)
                {
                    GameObject.Destroy(joint);
                }
            }
            
        }
    }
}