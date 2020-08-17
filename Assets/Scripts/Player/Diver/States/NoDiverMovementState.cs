using System.Collections;
using Core.State;

namespace Player.Diver
{
    public class NoDiverMovementState : IState 
    {
        public IEnumerator OnStateEnter()
        {
            yield break;
        }

        public IEnumerator OnStateExit()
        {
            yield break;
        }

        public void Tick()
        {
            
        }

        public void FixedTick()
        {
        }
    }
}