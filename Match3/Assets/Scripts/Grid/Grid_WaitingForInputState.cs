
using System.Collections.Generic;
using UnityEngine;

public partial class Grid
{
    public class WaitingForInputState : State
    {
        public override GridStateEnum CheckForNextState(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = statedMono as Grid;

            //Gestion state by message
            if (grid.frameDataBuffer.Exists<MessageData<Vector2>>((x) => x.message == "Generate"))
            {
                return (GridStateEnum.GENERATING_PIECES);
            }

            if (grid.frameDataBuffer.Exists<MessageData<List<Piece>>>((x) => x.message == "DeletePieces"))
            {
                return (GridStateEnum.DELETING_PIECES);
            }

            return this.stateType;
        }

        public override void OnEnter(StatedMono<GridStateEnum> statedMono)
        {
            //
        }

        public override void OnExit(StatedMono<GridStateEnum> statedMono)
        {
            //
        }

        public override void OnUpdate(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = statedMono as Grid;

            List<Piece> connections = grid.GetFirstPiecesConnection(3);
            if (connections != null)
            {
                grid.frameDataBuffer.AddData(new MessageData<List<Piece>>("DeletePieces", connections));
                return; //no player input authorized
            }
        }
    }

}
