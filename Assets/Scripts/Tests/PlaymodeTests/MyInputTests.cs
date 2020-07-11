using System.Collections;
using NUnit.Framework;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

namespace Tests.PlaymodeTests
{
    public class MyInputTests : InputTestFixture
    {
        public override void Setup()
        {
            base.Setup();
            InputSystem.AddDevice<Keyboard>();
            InputSystem.AddDevice<MyInputDevice>();
        }
        
        
        public class MyInputDevice : InputDevice
         {
             
         }
     }
 }