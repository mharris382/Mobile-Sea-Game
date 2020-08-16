using System;
using System.Linq;
using Diver;
using Player;
using UniRx;
using UnityEngine;
using Zenject;
using Holder = Holdables.Holder;

public class DiverHookCheck : MonoBehaviour
{
    private Holder _diverHolder;
    private InteractionTrigger _interactTrigger;
    private DiverInput _input;

    [Inject]
    void Inject(Holder holder, DiverInput input)
    {
        this._diverHolder = holder;
        
        this._input = input;
    }

    private void Awake()
    {
        _interactTrigger = GetComponentInChildren<InteractionTrigger>();
    }

    private void Start()
    {
        _input.OnUseHook.Subscribe(_ =>
        {
            var hookHolder = _interactTrigger.GetInRangeInteractables<HookHolder>().FirstOrDefault();
            if (hookHolder != null && hookHolder.Holder.HeldObject == null)
            {
                Debug.Log("Diver is in range of hook");
                if (_diverHolder.HeldObject != null)
                {
                    var held = _diverHolder.HeldObject;
                    _diverHolder.ReleaseHeldObject();
                    hookHolder.Holder.PickupObject(held);
                }
            }
        });
    }


}