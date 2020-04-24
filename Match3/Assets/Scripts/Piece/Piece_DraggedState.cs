
using UnityEngine;

public partial class Piece
{
    public class DraggedState : State
    {
        public override PieceStateEnum CheckForNextState(StatedMono<PieceStateEnum> statedMono)
        {
            Piece p = statedMono as Piece;
            if (p.frameDataBuffer.Exists<MessageData>((x) => x.message == ("BeReleased")))
                return PieceStateEnum.WAITING_FOR_INPUT;

            return this.stateType;
        }

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

        }
    }

}
