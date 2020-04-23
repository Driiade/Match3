using UnityEngine;

/// <summary>
/// Represent a piece on the grid
/// </summary>
public class Piece : StatedMono<PieceStateEnum>, IPositionProvider3D
{
    public class WaitingForInputState : State
    {
        public override void OnEnter(StatedMono<PieceStateEnum> statedMono)
        {
            //
        }

        public override void OnExit(StatedMono<PieceStateEnum> statedMono)
        {
            //
        }

        public override void OnUpdate(StatedMono<PieceStateEnum> statedMono)
        {
            //
        }
    }


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



}
