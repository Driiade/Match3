﻿using BC_Solution;
using UnityEngine;

/// <summary>
/// Represent a piece on the grid
/// </summary>
public partial class Piece : StatedMono<PieceStateEnum>, IPositionProvider3D, IAwakable
{
    [SerializeField]
    private PieceTypeEnum pieceType;
    public PieceTypeEnum PieceType
    {
        get => pieceType;
    }

    [SerializeField]
    PieceView pieceView;

    [SerializeField]
    FrameDataBufferMono frameDataBuffer;


    [SerializeField]
    PooledElement pooledElement;

    [SerializeField]
    AbstractClockMono clock;

    [SerializeField]
    AudioClip destroyAudioClip;

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
        Add(PieceStateEnum.BEING_DESTROYED, new BeingDestroyedState());
    }

    public void Select()
    {
        frameDataBuffer.AddData(new MessageData("BeTaken"));
    }

    public void Unselect()
    {
        frameDataBuffer.AddData(new MessageData("BeReleased"));
    }

    public void Destroy()
    {
        frameDataBuffer.AddData(new MessageData("Destroy")); //Can't be destroyed when taken. Maybe an issue later ?
    }

}
