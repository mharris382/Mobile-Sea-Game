using System.Collections;
using Core;
using NUnit.Framework;
using Player.Diver;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

namespace Tests.PlaymodeTests
{
    public class InputTests
    {
       

        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        [UnityTest]
        public IEnumerator InputsAreEnabled()
        {
            var diver = CreateDiver();
            yield return null;

            Assert.IsTrue( GameInput.DiverGameplayActions.Move.enabled, "Diver MoveAction not enabled");
            // Use the Assert class to test conditions.
            // yield to skip a frame
        }

        public Diver CreateDiver()
        {
            var go = new GameObject();
            return go.AddComponent<Diver>();
        }
    }

    
    
    
   
}