using UnityEngine;

/// <summary>
/// Interface for specifying Physics and view position (can be different for some applications)
/// </summary>
public interface IPositionProvider3D 
{
    Vector3 PhysicsPosition
    {
        get;
        set;
    }

    Vector3 ViewPosition
    {
        get;
        set;
    }
}
