using BC_Solution;
using System.Collections.Generic;
using UnityEngine;

public partial class Grid
{
    public class DeletingPiecesState : State
    {
        private List<Piece> pieceToDestroy;

        private float timer;
        private List<Vector2> positions;
        private int countCombo = 1;

        public override GridStateEnum CheckForNextState(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = (Grid)statedMono;

            if (pieceToDestroy.Count == 0)
            {
                return (GridStateEnum.GENERATING_NEW_PIECES);
            }

            return this.stateType;
        }

        public override void OnEnter(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = (Grid)statedMono;
            pieceToDestroy = grid.frameDataBuffer.GetLast<MessageData<List<Piece>>>((x) => x.message == "DeletePieces").obj;
            timer = grid.clock.CurrentRenderTime + 0.25f;

            positions = new List<Vector2>(pieceToDestroy.Count);
            for (int i = 0; i < pieceToDestroy.Count; i++)
            {
                positions.Add(pieceToDestroy[i].PhysicsPosition);
            }

            if(pieceToDestroy.Count >= 5)
                ServiceProvider.GetService<SFXSystem>().PlaySFX(grid.comboAudioClip);

            if (grid.LastState.stateType == GridStateEnum.GENERATING_NEW_PIECES)
                countCombo++;
            else
                countCombo = 1;

            ServiceProvider.GetService<ScoreSystem>().AddScore(pieceToDestroy.Count, countCombo);
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
                    for (int i = 0; i < pieceToDestroy.Count; i++)
                    {
                        pieceToDestroy[i].Destroy();
                    }

                    grid.frameDataBuffer.AddData(new MessageData<List<Vector2>>("GeneratePieces", positions));
                    pieceToDestroy.Clear();
                }
            }

        }
    }
}