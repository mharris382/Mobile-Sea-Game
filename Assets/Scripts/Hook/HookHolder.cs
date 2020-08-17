using System;
using Diver;
using Hook;
using Player;
using UniRx;
using UniRx.Diagnostics;
using UnityEngine;
using Zenject;
using Holder = Holdables.Holder;
using IHoldable = Holdables.IHoldable;

[RequireComponent(typeof(Rigidbody2D))]
public class HookHolder : MonoBehaviour, IInteractable
{
    private Rigidbody2D _rigidbody2D;
    private SpringJoint2D _springJoint2D;
    private DistanceJoint2D _distanceJoint2D;
    private DiverInput _input;

    public Rigidbody2D rigidbody2D
    {
        get => _rigidbody2D;
        set => _rigidbody2D = value;
    }

    [Inject] public Holder Holder { get; set; }


    [Inject]
    void Inject(DiverInput input)
    {
        this._input = input;
    }

    private void Awake()
    {
        _springJoint2D = GetComponent<SpringJoint2D>();
        _distanceJoint2D = GetComponent<DistanceJoint2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }
    
    private void Start()
    {
        _input.OnUseHook
            .Where(t => Holder.HeldObject != null)
            .Debug("Holder Will Try to Drop")
            .DelayFrame(1).Subscribe(_ =>
            {
                if (Holder.HeldObject != null)
                {
                    Holder.ReleaseHeldObject();
                }
            });
        MessageBroker.Default.Receive<HookHeldItemChangedSignal>()
            .Subscribe(t =>
            {
                if (t.HeldObject == null)
                {
                    ReleaseJoints();
                }
                else
                {
                    AttachJoints(t.HeldObject);
                }
            });
    }

    private void ReleaseJoints()
    {
        _distanceJoint2D.enabled = false;
        _springJoint2D.enabled = false;
        _springJoint2D.connectedBody = null;
        _distanceJoint2D.connectedBody = null;
    }

    private void AttachJoints(IHoldable objHeldObject)
    {
        _springJoint2D.connectedBody = objHeldObject.rigidbody2D;
        _distanceJoint2D.connectedBody = objHeldObject.rigidbody2D;
        _springJoint2D.connectedAnchor = Vector2.zero;
        _distanceJoint2D.connectedAnchor = Vector2.zero;
        _distanceJoint2D.enabled = true;
        _springJoint2D.enabled = true;
    }

    private void Update()
    {
        if (_distanceJoint2D.enabled)
        {
            _springJoint2D.connectedAnchor = Vector2.zero;
            _distanceJoint2D.connectedAnchor = Vector2.zero;
        }
    }
}