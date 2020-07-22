using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Holdables;
using NUnit.Framework;
using IInteractable=Player.IInteractable;
using IInteractionTrigger=Player.IInteractionTrigger;
using UnityEngine;
using UnityEngine.TestTools;
using HoldableObject = Holdables.HoldableObject;
using IHoldable = Holdables.IHoldable;
using Holdables;

namespace Tests.Holdables
{
    public class HoldableProviderTests
    {
        private Transform transform;
        
        [SetUp]
        public void setup()
        {
            transform = new GameObject().transform;
            transform.position = Vector2.zero;
        }

        [TearDown]
        public void cleanup()
        {
            if (transform != null) GameObject.DestroyImmediate(transform.gameObject);
        }
        
        [Test]
        public void ProviderReturnsNullWithNothingInRange()
        {
            HoldDummy holdDummy = new HoldDummy();
            InteractTriggerDummy triggerDummy = new InteractTriggerDummy();
            
            HoldableProvider provider = new HoldableProvider(holdDummy, triggerDummy, transform);
            Assert.IsNull(provider.GetFirstChoiceForPickup());
        }
        [Test]
        public void ProviderReturnsNullIfInRangeObjectIsBeingHeld()
        {
            HoldDummy holdDummy = new HoldDummy();
            holdDummy.HeldObject = CreateHoldableObject();
            InteractTriggerDummy triggerDummy = new InteractTriggerDummy(holdDummy.HeldObject);
            
            HoldableProvider provider = new HoldableProvider(holdDummy, triggerDummy, transform);
            Assert.IsNull(provider.GetFirstChoiceForPickup());
        }

        [Test]
        public void ProviderReturnsObjectInRange()
        {
            HoldDummy holdDummy = new HoldDummy();
            InteractTriggerDummy triggerDummy = new InteractTriggerDummy();
            
            triggerDummy.Interactables.Add(CreateHoldableObject());
            
            HoldableProvider provider = new HoldableProvider(holdDummy, triggerDummy, transform);
            Assert.IsNotNull(provider.GetFirstChoiceForPickup());
        }

        
        [Test]
        public void ProviderPrioritizesCloserObject()
        {
            HoldDummy holdDummy = new HoldDummy();
            InteractTriggerDummy triggerDummy = new InteractTriggerDummy();
            
            var closer = CreateHoldableObject(Vector2.zero);
            var farther = CreateHoldableObject(Vector2.one);
            
            triggerDummy.Interactables.Add(closer);
            triggerDummy.Interactables.Add(farther);
            
            HoldableProvider provider = new HoldableProvider(holdDummy, triggerDummy, transform);
            Assert.AreEqual(provider.GetFirstChoiceForPickup(), closer);
        }

        
        [Test]
        public void ProviderReturnsNotHeldObjectInRange()
        {
            HoldDummy holdDummy = new HoldDummy();
            InteractTriggerDummy triggerDummy = new InteractTriggerDummy();
            
            var closer = CreateHoldableObject(Vector2.zero);
            var farther = CreateHoldableObject(Vector2.one);
            
            triggerDummy.Interactables.Add(closer);
            triggerDummy.Interactables.Add(farther);
            holdDummy.HeldObject = closer;
            
            HoldableProvider provider = new HoldableProvider(holdDummy, triggerDummy, transform);
            Assert.AreEqual(provider.GetFirstChoiceForPickup(), farther);
        }

        private HoldableObject CreateHoldableObject()
        {
            return CreateHoldableObject(Vector2.zero);
        }

        HoldableObject CreateHoldableObject(Vector2 position)
        {
            var go  = new GameObject();
            var rb = go.AddComponent<Rigidbody2D>();
            var coll = go.AddComponent<CircleCollider2D>();
            go.transform.position = position;
            return go.AddComponent<HoldableObject>();
        }
        

        class HoldDummy : IHold
        {
            private IHoldable _heldObject;
            public event Action<IHoldable> OnPickedUp;
            public event Action<IHoldable> OnReleased;

            public IHoldable HeldObject
            {
                get => _heldObject;
                set => _heldObject = value;
            }


            public void Release()
            {
                OnReleased?.Invoke(HeldObject);
            }
        }



        class InteractTriggerDummy : IInteractionTrigger
        {
            public InteractTriggerDummy()
            {
            }
        
            public InteractTriggerDummy(params IInteractable[] interactables)
            {
                Interactables = new List<IInteractable>();
                Interactables.AddRange(interactables.Where(t => t!= null));
            }

            public List<IInteractable> Interactables { get; } = new List<IInteractable>();


            public List<T> GetInRangeInteractables<T>() where T : class, IInteractable => Interactables.OfType<T>().ToList();
        }
    }
}