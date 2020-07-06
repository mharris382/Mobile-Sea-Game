using NUnit.Framework;
using Player;
using UnityEngine;

namespace Tests.PlaymodeTests
{
    public class HookTests
    {
        
        // [NUnit.Framework.Test]
        // public void HookTestsSimplePasses()
        // {
        //     // Use the Assert class to test conditions.
        //     
        // }
        [NUnit.Framework.Test]
        public void ObjectCanBePickedUpBy()
        {
            var holder = CreateHolder();
            var obj = CreateHeavyHoldableObject(Vector2.zero);
            
            Assert.IsTrue(obj.CanBePickedUpBy(holder), "HoldableObject cannot be picked up by holder");
        }
        
        

        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        // [UnityEngine.TestTools.UnityTest]
        // public System.Collections.IEnumerator HookTestsWithEnumeratorPasses()
        // {
        //     // Use the Assert class to test conditions.
        //     // yield to skip a frame
        //     yield return null;
        // }

        public Holder CreateHolder()
        {
            var go = new GameObject();
            return go.AddComponent<Holder>();
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
            go.layer = 1 << 10;
            return edgeCollider;
        }

        public BuoyancyEffector2D CreateWaterBuoyancyEffector2D()
        {
            var go = new GameObject();
            go.transform.position = Vector2.zero;
            var rb = go.AddComponent<Rigidbody2D>();
            rb.isKinematic = true;
            var box = go.AddComponent<BoxCollider2D>();
            box.size = Vector2.one * 10;
            box.offset = new Vector2(0, box.size.y/2f);
            box.isTrigger = true;
            box.usedByEffector = true;
            return go.AddComponent<BuoyancyEffector2D>();
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

        public IDetectInteractions CreateInteractionTrigger(Vector2 position)
        {
            var go = new GameObject();
            go.transform.position = position;
            var rb = go.AddComponent<Rigidbody2D>();
            var coll = go.AddComponent<CircleCollider2D>();
            coll.isTrigger = true;
            coll.radius = 1;
            return go.AddComponent<InteractionTrigger>();
        }
    }
}