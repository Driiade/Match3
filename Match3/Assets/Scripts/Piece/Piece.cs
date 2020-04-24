using UnityEngine;

/// <summary>
/// Represent a piece on the grid
/// </summary>
public partial class Piece : StatedMono<PieceStateEnum>, IPositionProvider3D, IStartable
{
    [SerializeField]
    private PieceTypeEnum pieceType;
    public PieceTypeEnum PieceType
    {
        get => pieceType;
    }


    private Vector2 index;

    /// <summary>
    /// PhysicsPosition and ViewPosition are not the same. Piece are attached "Physically" on grid index
    /// </summary>
    public Vector3 PhysicsPosition { get => index; set => index = value; }
    public Vector3 ViewPosition { get => this.transform.position; set => this.transform.position = value; }

    public void IStart()
    {
        //StartBehaviour(PieceStateEnum.APPEARING);
    }
}
