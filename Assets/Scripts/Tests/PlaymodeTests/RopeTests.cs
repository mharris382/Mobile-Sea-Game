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
        public  void RopePrefabExists()
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
        
        
    }
}