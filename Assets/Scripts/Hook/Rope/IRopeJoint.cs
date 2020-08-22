using UnityEngine;

public interface IRopeJoint
{
    IRopeJoint NextJoint { get; }
    IRopeJoint PrevJoint { get; }
    
    Vector2 ConnectedAnchor { get; set; }
    Vector2 Anchor { get; set; }
    Rigidbody2D ConnectedBody { get; set; }
    Rigidbody2D AttachedBody { get; }
    float GetTotalDistance();
    void SetDistance(float target);
}