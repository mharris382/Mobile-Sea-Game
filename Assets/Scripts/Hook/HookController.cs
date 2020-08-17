

using System;
using UnityEngine;
using NPBehave;


namespace Hook
{
    
    
    public class HookController : MonoBehaviour
    {
        
        /*
         -> [NOT SPAWNED] 
            ->  is hook spawned {SEQ}
               -> hook is holding treasure? => COLLECT TREASURE
               -> is diver holding hook => SEND MESSAGE TO TRY EXIT LEVEL TO BOAT
                  
            -> goto [NOT SPAWNED]
            -> wait for spawn input
            -> check if spawn position is valid
            -> SPAWN HOOK (enable hook GameObject)
            -> set spawned to true
            
         -> [SPAWNED] {SEL}
                
            -> RAISE HOOK {SEQ}
                -> check for call hook input => stop raising hook
                -> triggered by raise input 
                -> reached surface? => set isSpawned => false
                    
            -> HELD BY DIVER {SEL}
                -> is hook attached to object?  {SEL}
                    -> check for detach input  {SEQ}
                        - DROP OBJECT
                        - wait for a second
                    -> move object with diver provided it is a valid position
                -> check for attach input?
                    => try attach hook to valid object            
                    
            -> MOVE HOOK TO TARGET  {SEL}
                -> requested target position is valid 
                    -> is hook very far away (distance from diver > jumpThreshold) 
                        (aka would it take an annoying amount of time to reach the diver's position and break the player's flow)
                        -> wait for a few seconds
                        -> move hook to just outside screen
6                    -> has los to diver 
                        -> has los to surface?
                        -> move in straight line to diver
                    -> diver has los to surface
                    
         
        */
        
    }
    
    
    
}
