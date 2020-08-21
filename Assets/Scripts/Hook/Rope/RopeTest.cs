using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hook.Rope;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

public class RopeTest : MonoBehaviour , IEnumerable<RopeTest.RopeJoint>
{
    public int startLength = 5;
    [SerializeField, MaxValue("maxDistance")] private float minDistance = 0.1f;
    [SerializeField, MinValue("minDistance")] private float maxDistance = 50;
    
    [Required, AssetsOnly] public PooledRopeJoint poolPrefab;
    [Required,SceneObjectsOnly]public Rigidbody2D connected;
    
    
    
    
    private RopeJoint _head;
    private RopeJoint _first;
    private IFactory<RopeJoint, RopeJoint> _factory;

    public IFactory<RopeJoint, RopeJoint> Factory => _factory;

    #region [Editor Only]

#if UNITY_EDITOR
    void Plus1() => Distance += 0.1f;

    void Minus1() => Distance -= 0.1f;
#endif



    [InlineButton("Plus1", "+"), InlineButton("Minus1", "-")]
    [ShowInInspector, HideInEditorMode, MinValue("minDistance")]
    #endregion
    public float Distance
    {
        get => GetDistance();
        set => SetDistance(value);
    }
    
    public IEnumerable<RopeJoint> AllJoints => this;
    
    public IEnumerable<Vector2> JointPositions => this.Select(t => t.AttachedBody.position);
    // private ReactiveProperty<float> _distance = new ReactiveProperty<float>();
 


    public void Awake()
    {
        // var joints = GetComponentsInChildren<PooledRopeJoint>();
        // if (joints == null || joints.Length == 0)
        // {
        //     Debug.LogError("No child Distance Joints", this);
        //     return;
        // }

        this._factory = new RopeTest.RopeJointFactory(poolPrefab,transform, connected);
        _first = _factory.Create(null);
        _head = _first;
        _head.ConnectedBody = connected;
        // _first = new RopeJoint(joints[0], null, _factory);
        // _head = _first;
        // for (int i = 1; i < joints.Length; i++)
        // {
        //     _head.NextJoint = new RopeJoint(joints[i], _head, _factory);
        //     _head = _head.NextJoint;
        // }
        //
        // minDistance = _first.GetTotalDistance();
        // _head.ConnectedBody = connected;
        // _distance.Value = _minDistance;
    }

    private void Start()
    {
     
        Distance = startLength;
    }

    private void SetDistance(float dist)
    {
        if (_first == null) Debug.LogError("Missing First");
        dist = Mathf.Clamp(dist, minDistance, maxDistance);
        _first.SetDistance(dist);
        // _distance.Value = dist;
    }

    private float GetDistance()
    {
        if (_first == null) return -1;
        return _first.GetTotalDistance();
    }


    private void LateUpdate()
    {
        foreach (var joint in this)
        {
            joint.Anchor = Vector2.zero;
            joint.ConnectedAnchor = Vector2.zero;
            
        }
    }
    public IEnumerator<RopeJoint> GetEnumerator()
    {
        var head = _first;
        while (head.NextJoint != null)
        {
            yield return head;
            head = head.NextJoint;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #region [Joint Classes]

   

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

    #region Obsolete

    // [System.Obsolete("use PooledFactory instead")]
    // public class Factory : IFactory<RopeJoint, RopeJoint>
    // {
    //     public RopeJoint Create(RopeJoint joint)
    //     {
    //         var distanceJoint2D = joint._joint;
    //         var transform1 = distanceJoint2D.transform;
    //         var newDistanceJoint = Instantiate(distanceJoint2D, distanceJoint2D.attachedRigidbody.position, transform1.rotation, transform1.parent);
    //         var newJoint = new RopeJoint(newDistanceJoint, joint, this) {_prevJoint = joint};
    //         return newJoint;
    //     }
    // }

    // public class Pool : Factory, IMemoryPool<RopeJoint>
    // {
    //     private int _numTotal;
    //     private int _numActive;
    //     private int _numInactive;
    //     private Type _itemType;
    //     public void Despawn(RopeJoint item)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public RopeJoint Spawn()
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public int NumTotal
    //     {
    //         get => _numTotal;
    //         set => _numTotal = value;
    //     }
    //
    //     public int NumActive
    //     {
    //         get => _numActive;
    //         set => _numActive = value;
    //     }
    //
    //     public int NumInactive
    //     {
    //         get => _numInactive;
    //         set => _numInactive = value;
    //     }
    //
    //     public Type ItemType
    //     {
    //         get => _itemType;
    //         set => _itemType = value;
    //     }
    //
    //     public void Resize(int desiredPoolSize)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public void Clear()
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public void ExpandBy(int numToAdd)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public void ShrinkBy(int numToRemove)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public void Despawn(object obj)
    //     {
    //         throw new NotImplementedException();
    //     }
    // }
    // [System.Obsolete]
    // public class FirstRopeJoint : RopeJoint , IEnumerable<RopeJoint>
    // {
    //     [System.Obsolete]
    //     public FirstRopeJoint(DistanceJoint2D joint) : base(joint, null)
    //     {
    //     }
    //     public FirstRopeJoint(PooledRopeJoint joint,IFactory<RopeJoint, RopeJoint> factory) : base( null,joint, factory)
    //     {
    //     }
    //
    //     public float GetTotalDistance()
    //     {
    //         float curr = 0;
    //         return base.GetTotalDistance(ref curr);
    //     }
    //
    //     public void SetDistance(float target)
    //     {
    //         float curr = 0;
    //         base.SetDistance(target, ref curr);
    //     }
    //
    //     public IEnumerator<RopeJoint> GetEnumerator()
    //     {
    //         RopeJoint curr = this;
    //         while (curr != null)
    //         {
    //             yield return curr;
    //             curr = curr.NextJoint;
    //         }
    //     }
    //     public IEnumerable<RopeJoint> GetEnumerable()
    //     {
    //         RopeJoint curr = this;
    //         while (curr != null)
    //         {
    //             yield return curr;
    //             curr = curr.NextJoint;
    //         }
    //     }
    //
    //     protected override void DestroySelf()
    //     {
    //         base.DestroySelf();
    //     }
    //
    //     IEnumerator IEnumerable.GetEnumerator()
    //     {
    //         return GetEnumerator();
    //     }
    //     
    //     
    //     
    //     
    // }

    #endregion

    #endregion


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

public interface IRopeJoint
{
    IRopeJoint NextJoint { get; }
    IRopeJoint PrevJoint { get; }
    
    Vector2 ConnectedAnchor { get; set; }
    Vector2 Anchor { get; set; }
    Rigidbody2D ConnectedBody { get; set; }
    Rigidbody2D AttachedBody { get; }
    float GetTotalDistance();
    void SetDistance(float target);
}


