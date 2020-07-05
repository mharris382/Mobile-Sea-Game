using System;
using System.Collections;
using NUnit.Framework;
using Player.Diver;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlaymodeTests
{
    public class DepthTests
    {
        [UnityTest]
        public IEnumerator DepthTrackerReturnsCorrectDepth()
        {
            // Use the Assert class to test conditions.
            var pos = new Vector2(0, -10f);
            var tracker = CreateDepthTracker(pos);
            yield return null;
            Assert.AreEqual(10, tracker.CurrentDepth.feetBelowSeaLvl);
            tracker.transform.position += (Vector3.down * 5);
            yield return null;
            Assert.AreEqual(10 + 5, tracker.CurrentDepth.feetBelowSeaLvl);
            
        }
        
        
        [UnityTest]

        public IEnumerator DepthTrackerPublishesEventsCorrectly()
        {
            var pos = new Vector2(0, -10f);
            var tracker = CreateDepthTracker(pos);
            var velocity = Vector3.down;
            
            int? recieved = null;

            void TrackerOnOnDepthChanged(Depth depth)
            {
                recieved = depth;
            }

            tracker.OnDepthChanged += TrackerOnOnDepthChanged;
            tracker.transform.position += velocity;
            
            yield return null;
            Assert.NotNull(recieved, "Depth Event was not published");
            Assert.AreEqual(11, tracker.CurrentDepth.feetBelowSeaLvl);
            
            tracker.transform.position += velocity;
            yield return null;
            Assert.AreEqual(12, tracker.CurrentDepth.feetBelowSeaLvl);
            tracker.OnDepthChanged -= TrackerOnOnDepthChanged;
        }


        DepthTracker CreateDepthTracker(Vector2 pos)
        { 
            var go = new GameObject();
            go.transform.position = pos;
            return go.AddComponent<DepthTracker>();
        }
    }
}