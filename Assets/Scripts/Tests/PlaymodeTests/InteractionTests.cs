using System.Collections;
using NUnit.Framework;
using Player;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlaymodeTests
{
    public class InteractionTests
    {
        [Test]
        public void InteractionTestsSimplePasses()
        {
            // Use the Assert class to test conditions.
            
        }

        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        [UnityTest]
        public IEnumerator InteractionTriggerDetectsInRangeHoldables()
        {
            var interactionTrigger = CreateInteractionTrigger(Vector2.zero);
            var holdable = CreateHeavyHoldableObject(Vector2.zero);
            // Assert.Less(Vector2.Distance(holdable.rigidbody2D.position, interactionTrigger.transform.position), interactionTrigger.GetComponent<CircleCollider2D>().radius);
            CreateSeaFloor(10, 10, -0.5f);
            yield return null;
            
            
            Assert.IsTrue(interactionTrigger.HasTypeInRange<IHoldable>());
            
            var inRange = interactionTrigger.GetInRangeInteractables<IHoldable>();
            Assert.IsNotEmpty(inRange);
            Assert.Contains(holdable, inRange);
            
        }
        
        [UnityTest]
        public IEnumerator InteractionTriggerDoesntDetectsOutOfRangeHoldables()
        {
            var interactionTrigger = CreateInteractionTrigger(Vector2.right * 3);
            CreateSeaFloor(10, 10, -0.5f);
            var holdable = CreateHeavyHoldableObject(Vector2.zero);
            // Assert.Greater(Vector2.Distance(holdable.rigidbody2D.position, interactionTrigger.transform.position), interactionTrigger.GetComponent<CircleCollider2D>().radius);
            yield return null;
            
            
            Assert.IsFalse(interactionTrigger.HasTypeInRange<IHoldable>());
            Assert.IsEmpty(interactionTrigger.GetInRangeInteractables<IHoldable>());
        }
        
        public EdgeCollider2D CreateSeaFloor(float xMin, float xMax, float yMin)
        {
            var go = new GameObject();
            var edgeCollider = go.AddComponent<EdgeCollider2D>();
            Vector2[] pnts = new[]
            {
                new Vector2(xMin, yMin),
                new Vector2(xMax, yMin),
            };
            edgeCollider.points = pnts;
            //go.layer = 1 << 10;
            return edgeCollider;
        }
        public InteractionTrigger CreateInteractionTrigger(Vector2 position)
        {
            var go = new GameObject();
            go.transform.position = position;
            var rb = go.AddComponent<Rigidbody2D>();
            var coll = go.AddComponent<CircleCollider2D>();
            rb.isKinematic = true;
            coll.isTrigger = true;
            coll.radius = 1;
            return go.AddComponent<InteractionTrigger>();
        }
        
        
        public HoldableObject CreateHeavyHoldableObject(Vector2 position)
        {
            var go = new GameObject();
            go.transform.position = position;
            var rb = go.AddComponent<Rigidbody2D>();
            var box = go.AddComponent<BoxCollider2D>();
            rb.useAutoMass = true;
            rb.isKinematic = false;
            box.density = 5;
            box.size = Vector2.one;
            box.offset = Vector2.zero;
            return go.AddComponent<HoldableObject>();
        }
    }
}