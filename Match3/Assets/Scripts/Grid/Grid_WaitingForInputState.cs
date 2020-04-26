
using System.Collections.Generic;
using UnityEngine;

public partial class Grid
{
    public class WaitingForInputState : GridState
    {
        public override Piece AskForAPiece(Grid grid, Vector2 position)
        {
            if (grid.GetFirstPiecesConnection(3) == null) //The only state where nothing is done, and all piece are in place
            {
                position = grid.WorldToGridPosition(position);
                if (position.x >= 0 && position.x <= grid.size.x && position.y >= 0 && position.y <= grid.size.y)
                {
                    return grid.gridPieces[(int)position.x][(int)position.y];
                }
            }

            return null;
        }

        public override GridStateEnum CheckForNextState(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = statedMono as Grid;

            if (grid.frameDataBuffer.Exists<MessageData<Piece>>((x) => x.message == "PieceTaken")) //This is the most important, let player play
            {
                return (GridStateEnum.PIECE_BEING_DRAGGED);
            }

            //Gestion state by message
            if (grid.frameDataBuffer.Exists<MessageData<Vector2>>((x) => x.message == "Generate"))
            {
                return (GridStateEnum.GENERATING_PIECES);
            }

            if (grid.frameDataBuffer.Exists<MessageData<List<Piece>>>((x) => x.message == "DeletePieces"))
            {
                return (GridStateEnum.DELETING_PIECES);
            }

            if (grid.frameDataBuffer.Exists<MessageData<Piece>>((x) => x.message == "PlacePiece"))
            {
                return (GridStateEnum.SWITCHING_PIECES);
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
            }
        }
    }

}
