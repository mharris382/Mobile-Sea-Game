using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlaymodeTests
{
    [TestFixture]
    public class RopeTests
    {
        [NUnit.Framework.Test]
        public void RopePrefabExists()
        {
            var prefab = Resources.Load<RopeTest>("RopePrefab");

            Assert.NotNull(prefab, "No RopePrefab resource found");
        }


        [UnityTest]
        public IEnumerator RopeDistanceIncreases()
        {
            var prefab = Resources.Load<RopeTest>("RopePrefab");
            var spawned = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            Assert.NotNull(spawned.connected, "No connected rigidbody at end of rope");
            yield return null;


            int cnt = spawned.AllJoints.Count();
            Assert.Greater(cnt, 2);

            spawned.Distance += 1;
            Assert.Greater(spawned.AllJoints.Count(), cnt);

            spawned.Distance -= 1;
            Assert.AreEqual(cnt, spawned.AllJoints.Count());
        }

        [NUnit.Framework.Test]
        public void JointFactoryCreatesFirstJoint()
        {
            var parent = new GameObject();
            var prefab = Resources.Load<RopeTest>("RopePrefab").poolPrefab;
            var connected = new GameObject().AddComponent<Rigidbody2D>();
            var factory = new RopeTest.RopeJointFactory(prefab, parent.transform, connected);
            var first = factory.Create(null);
            Assert.NotNull(first);
            Assert.IsNull(first.PrevJoint);
            Assert.IsNull(first.NextJoint);
            Assert.AreEqual(first.ConnectedBody, connected);
        }


        [NUnit.Framework.Test]
        public void JointFactoryCreatesMultipleJointsCorrectly()
        {
            var parent = new GameObject();
            var prefab = Resources.Load<RopeTest>("RopePrefab").poolPrefab;
            var connected = new GameObject().AddComponent<Rigidbody2D>();
            var factory = new RopeTest.RopeJointFactory(prefab, parent.transform, connected);
            var first = factory.Create(null);
            var second = factory.Create(first);
            Assert.AreEqual(first.ConnectedBody, second.AttachedBody, "First joint is not connected to second joint");
            Assert.AreEqual(second.ConnectedBody, connected, "Second Joint is not connected to end joint");
        }

        [UnityTest]
        public IEnumerator EnumeratorWorksCorrectly()
        {
            var prefab = Resources.Load<RopeTest>("RopePrefab");
            var spawned = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            Assert.NotNull(spawned.connected, "No connected rigidbody at end of rope");
            yield return null;
            int count = 0;
            do
            {
                foreach (var joint in spawned) count++;
                spawned.Distance += 1;
                yield return new WaitForSeconds(0.125f);
            } while (count < 20);
            

            RopeTest.RopeJoint[] joints = spawned.ToArray();  
            RopeTest.RopeJoint prev = joints[0];
            
            for (int i = 1; i < count; i++)
            {
                var next = joints[i];
                Assert.AreEqual(prev , next.PrevJoint);
                Assert.AreEqual(next, prev.NextJoint);
                Assert.AreEqual(prev.ConnectedBody, next.AttachedBody);
                prev = next;
            }
            Assert.AreEqual(prev.ConnectedBody, spawned.connected);
        }
    }
}