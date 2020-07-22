using System.Collections.Generic;

namespace Player
{
    [System.Obsolete("Now use IInteractionTrigger")]
    public interface IDetectInteractions
    {
        List<T> GetInRangeInteractables<T>() where T : class, IInteractable;
    }
}