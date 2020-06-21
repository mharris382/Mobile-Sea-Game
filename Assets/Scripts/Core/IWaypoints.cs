using UnityEngine;

namespace Core
{
    public interface IWaypoints
    {
        Transform[] WaypointTransforms { get; set; }
    }
}