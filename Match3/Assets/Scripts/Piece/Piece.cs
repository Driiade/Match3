using UnityEngine;

/// <summary>
/// Represent a piece on the grid
/// </summary>
public class Piece : StatedMono<PieceState>, IPositionProvider3D
{
    [SerializeField]
    private PieceType pieceType;
    public PieceType PieceType
    {
        get => pieceType;
    }

    /// <summary>
    /// PhysicsPosition and ViewPosition are the same in this applicaiton, for simplification.
    /// </summary>
    public Vector3 PhysicsPosition { get => this.transform.position; set => this.transform.position = value; }
    public Vector3 ViewPosition { get => this.transform.position; set => this.transform.position = value; }
}
