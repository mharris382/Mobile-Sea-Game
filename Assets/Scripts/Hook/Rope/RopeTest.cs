using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hook.Rope;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

public partial class RopeTest : MonoBehaviour , IEnumerable<IRopeJoint>
{
    public float startLength = 5;
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
    
    public IEnumerable<IRopeJoint> AllJoints => this;
    
    public IEnumerable<Vector2> JointPositions => this.Select(t => t.AttachedBody.position);
    // private ReactiveProperty<float> _distance = new ReactiveProperty<float>();
 


    public void Awake()
    {
        this._factory = new RopeJointFactory(poolPrefab,transform, connected);
        _first = _factory.Create(null);
        _head = _first;
        _head.ConnectedBody = connected;
    }

    private void Start()
    {
     
        Distance = startLength;
    }

    private void SetDistance(float dist)
    {
        if (_first == null)
        {
            Debug.LogError("Missing First");
            return;
        }
        dist = Mathf.Clamp(dist, minDistance, maxDistance);
        _first.SetDistance(dist);
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
    public IEnumerator<IRopeJoint> GetEnumerator()
    {
        var head = _first;
        while (head != null)
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
}