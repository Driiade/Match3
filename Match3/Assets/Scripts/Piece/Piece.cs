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

    /// <summary>
    /// PhysicsPosition and ViewPosition are the same in this applicaiton, for simplification.
    /// </summary>
    public Vector3 PhysicsPosition { get => this.transform.position; set => this.transform.position = value; }
    public Vector3 ViewPosition { get => this.transform.position; set => this.transform.position = value; }



}
