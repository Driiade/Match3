using BC_Solution;
using UnityEngine;

public partial class Player : StatedMono<PlayerStateEnum>, IAwakable, IStartable
{
    [SerializeField]
    FrameDataBufferMono frameDataBuffer;

    [SerializeField]
    AbstractClockMono clock;

    public void IAwake()
    {
        //On Separate file
        Add(PlayerStateEnum.WAITING_FOR_INPUT, new WaitingForInputState());
        Add(PlayerStateEnum.DRAGGING_PIECE, new DraggingPieceState());
        //
    }

    public void IStart()
    {
        StartBehaviour(PlayerStateEnum.WAITING_FOR_INPUT);
    }
}
