using System.Collections;

namespace Core.State
{
    public interface IState
    {
        IEnumerator OnStateEnter();
        IEnumerator OnStateExit();
        void Tick();
        void FixedTick();
    }
}

