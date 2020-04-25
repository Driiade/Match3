using BC_Solution;
using System.Collections.Generic;
using UnityEngine;

public partial class Grid
{
    public class GeneratingPiecesState : State
    {
        float startTime;

        public override GridStateEnum CheckForNextState(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = (Grid)statedMono;
            float deltaTime = grid.clock.CurrentRenderTime - startTime;
            if (deltaTime > 2f)
            {
               return (GridStateEnum.WAITING_FOR_INPUT);
            }
            else
                return this.stateType;
        }

        public override void OnEnter(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = (Grid)statedMono;
            Vector2 size = grid.frameDataBuffer.GetLast<MessageData<Vector2>>((x) => x.message == "Generate").obj;
            grid.size = size;
            grid.gridPieces = ArrayExtension.New2DArray<Piece>((int)size.x, (int)size.y);

            for (int i = 0; i < grid.piecePools.Length; i++)
            {
                grid.piecePools[i].objectPool.PoolAll();
            }
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    grid.Populate(i, j);
                }
            }

            grid.gridView.Initialize(grid.size);

            startTime = grid.clock.CurrentRenderTime;
        }

        public override void OnExit(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = (Grid)statedMono;
            for (int i = 0; i < grid.size.x; i++)
            {
                for (int j = 0; j < grid.size.y; j++)
                {
                    grid.gridPieces[i][j].ViewPosition = new Vector3(i, -j, grid.ViewPosition.z); //Starting to left top
                }
            }
        }


        /// <summary>
        /// Main purpose: Add a small effect when generating piece
        /// </summary>
        /// <param name="statedMono"></param>
        public override void OnUpdate(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = (Grid)statedMono;
            float deltaTime = grid.clock.CurrentRenderTime - startTime;

            for (int i = 0; i < grid.size.x; i++)
            {
                for (int j = 0; j < grid.size.y; j++)
                {
                    grid.gridPieces[i][j].ViewPosition = new Vector3(i, Mathf.Max(-j, -j + 6f * Mathf.Sin(i * 0.2f) - deltaTime * 10f + grid.size.y), grid.ViewPosition.z); //Starting to left top

                    if (grid.gridPieces[i][j].ViewPosition.y > grid.ViewPosition.y + 1f)
                        grid.gridPieces[i][j].gameObject.SetActive(false);
                    else
                        grid.gridPieces[i][j].gameObject.SetActive(true);
                }
            }
        }
    }
}
