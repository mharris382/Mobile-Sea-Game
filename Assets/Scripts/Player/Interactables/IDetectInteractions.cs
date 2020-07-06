using System.Collections.Generic;

namespace Player
{
    public interface IDetectInteractions
    {
        List<T> GetInRangeInteractables<T>() where T : class, IInteractable;
    }
}