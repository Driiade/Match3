
public partial class Piece
{
    public class WaitingForInputState : State
    {
        public override PieceStateEnum CheckForNextState(StatedMono<PieceStateEnum> statedMono)
        {
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
            //
        }
    }

}
