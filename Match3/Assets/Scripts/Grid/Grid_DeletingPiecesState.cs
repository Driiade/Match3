using BC_Solution;
using System.Collections.Generic;
using UnityEngine;

public partial class Grid
{
    public class DeletingPiecesState : State
    {
        private List<Piece> pieceToDestroy;

        private float timer;

        public override GridStateEnum CheckForNextState(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = (Grid)statedMono;

            if (grid.frameDataBuffer.Exists<MessageData<List<Vector2>>>((x) => x.message == "GeneratePieces"))
            {
                return (GridStateEnum.GENERATING_NEW_PIECES);
            }

            return this.stateType;
        }

        public override void OnEnter(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = (Grid)statedMono;
            pieceToDestroy = grid.frameDataBuffer.GetLast<MessageData<List<Piece>>>((x) => x.message == "DeletePieces").obj;
            timer = grid.clock.CurrentRenderTime + 0.5f;
        }

        public override void OnExit(StatedMono<GridStateEnum> statedMono)
        {

        }

        public override void OnUpdate(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = (Grid)statedMono;


            if (pieceToDestroy.Count > 0)
            {
                if (grid.clock.CurrentRenderTime > timer)
                {
                    List<Vector2> positions = new List<Vector2>(pieceToDestroy.Count);
                    for (int i = 0; i < pieceToDestroy.Count; i++)
                    {
                        pieceToDestroy[i].GetComponent<PooledElement>().Pool();
                        positions.Add(pieceToDestroy[i].PhysicsPosition);
                    }

                    pieceToDestroy.Clear();

                    grid.frameDataBuffer.AddData(new MessageData<List<Vector2>>("GeneratePieces", positions));
                }
            }

        }
    }
}
