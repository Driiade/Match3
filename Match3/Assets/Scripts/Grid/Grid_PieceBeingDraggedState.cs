﻿
using System.Collections.Generic;
using UnityEngine;

public partial class Grid
{
    public class PieceBeingDraggedState : State
    {

        Piece draggedPiece;
        List<Vector2> highlightList = new List<Vector2>();

        public override GridStateEnum CheckForNextState(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = statedMono as Grid;
            if (grid.frameDataBuffer.Exists<MessageData<Piece>>((x) => x.message == "PlacePiece"))
            {
                return (GridStateEnum.SWITCHING_PIECES);
            }

            if (draggedPiece.CurrentStateType != PieceStateEnum.DRAGGED)
                return GridStateEnum.WAITING_FOR_INPUT;

            return this.stateType;
        }

        public override void OnEnter(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = statedMono as Grid;
            draggedPiece = (grid.frameDataBuffer.GetLast<MessageData<Piece>>((x) => x.message == "PieceTaken")).obj;

            highlightList.Clear();

            Vector2 r = new Vector2(draggedPiece.PhysicsPosition.x + 1, draggedPiece.PhysicsPosition.y);

            if(r.x < grid.Size.x)
                highlightList.Add(r);

            Vector2 l = new Vector2(draggedPiece.PhysicsPosition.x - 1, draggedPiece.PhysicsPosition.y);

            if (l.x >= 0)
                highlightList.Add(l);

            Vector2 t = new Vector2(draggedPiece.PhysicsPosition.x,  draggedPiece.PhysicsPosition.y +1);

            if (t.y < grid.Size.y)
                highlightList.Add(t);

            Vector2 b = new Vector2(draggedPiece.PhysicsPosition.x, draggedPiece.PhysicsPosition.y -1);

            if (b.y >=0)
                highlightList.Add(b);

            grid.gridView.StartHighlighting(highlightList);

        }

        public override void OnExit(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = statedMono as Grid;
            grid.gridView.StopHighlighting(highlightList);
        }

        public override void OnUpdate(StatedMono<GridStateEnum> statedMono)
        {

        }
    }

}