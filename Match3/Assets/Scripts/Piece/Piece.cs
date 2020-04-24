using BC_Solution;
using UnityEngine;

/// <summary>
/// Represent a piece on the grid
/// </summary>
public partial class Piece : StatedMono<PieceStateEnum>, IPositionProvider3D, IStartable, IAwakable
{
    [SerializeField]
    private PieceTypeEnum pieceType;
    public PieceTypeEnum PieceType
    {
        get => pieceType;
    }

    [SerializeField]
    FrameDataBufferMono frameDataBuffer;

    private Vector2 index;

    /// <summary>
    /// PhysicsPosition and ViewPosition are not the same. Piece are attached "Physically" on grid index
    /// </summary>
    public Vector3 PhysicsPosition { get => index; set => index = value; }
    public Vector3 ViewPosition { get => this.transform.position; set => this.transform.position = value; }

    public void IAwake()
    {
        Add(PieceStateEnum.WAITING_FOR_INPUT, new WaitingForInputState());
        Add(PieceStateEnum.DRAGGED, new DraggedState());
    }

    public void IStart()
    {
        StartBehaviour(PieceStateEnum.WAITING_FOR_INPUT);
    }

    public void BeTaken()
    {
        frameDataBuffer.AddData(new MessageData("BeTaken"));
    }

    public void BeReleased()
    {
        frameDataBuffer.AddData(new MessageData("BeReleased"));
    }

}
