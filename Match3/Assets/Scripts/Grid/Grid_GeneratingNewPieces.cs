using System.Collections.Generic;
using UnityEngine;

public partial class Grid
{
    public class GeneratingNewPieces : State
    {
        List<Vector2> missingPosition;
        List<Piece> piecesToMove = new List<Piece>();

        float yVelocity;
        bool allPiecesFall = false;

        public float gravity = -9.18f;

        public override void OnEnter(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = (Grid)statedMono;
            allPiecesFall = false;
            missingPosition = grid.frameDataBuffer.GetLast<MessageData<List<Vector2>>>((x) => x.message == "GeneratePieces").obj;
            piecesToMove.Clear();

            //Ok now count what is missing per column
            int[] missingPiecePerColumn = new int[(int)grid.size.x];
            int[] maxYPerColumn = new int[(int)grid.size.x];

            for (int i = 0; i < missingPosition.Count; i++)
            {
                int x = (int)missingPosition[i].x;
                missingPiecePerColumn[x] += 1;
                maxYPerColumn[x] = Mathf.Max(maxYPerColumn[x], (int)missingPosition[i].y);
            }



            for (int i = 0; i < missingPiecePerColumn.Length; i++)  //Just place them in the right place in the array. View position will be placed later.
            {
                int maxY = maxYPerColumn[i];
                for (int k = 0; k < missingPiecePerColumn[i]; k++)
                {
                    for (int j = maxY; j >= 1; j--)
                    {
                        grid.InterChange(grid.gridPieces[i][j - 1], grid.gridPieces[i][j]);
                    }

                    Piece p = grid.Populate(i, 0);
                    p.ViewPosition = new Vector2(i, (k + 1));
                }

                for (int j = 0; j < maxYPerColumn[i] + 1; j++)
                {
                    piecesToMove.Add(grid.gridPieces[i][j]);
                }
            }

            yVelocity = 0f;
        }

        public override void OnExit(StatedMono<GridStateEnum> statedMono)
        {
            //Just replace all pieces, jsut in case
            Grid grid = (Grid)statedMono;
            foreach (var item in piecesToMove)
            {
                item.ViewPosition = new Vector2(item.PhysicsPosition.x, -item.PhysicsPosition.y);
                item.gameObject.SetActive(true);
            }
        }


        public override void OnUpdate(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = (Grid)statedMono;

            float deltaTime = grid.clock.DeltaRenderTime;
            float gt = gravity * deltaTime;
            yVelocity += gt;
            gt *= 0.5f * deltaTime;

            float deltaPosition = yVelocity * deltaTime + gt; //Cache this

            bool stillFalling = false;
            foreach (var p in piecesToMove)
            {
                Vector2 targetPosition = p.PhysicsPosition;
                targetPosition.y *= -1f;

                p.ViewPosition = new Vector2(p.ViewPosition.x, Mathf.Max(targetPosition.y, p.ViewPosition.y + deltaPosition));

                if (!stillFalling && p.ViewPosition.y - targetPosition.y >= 0.01f)
                    stillFalling = true;

                if (p.ViewPosition.y > grid.ViewPosition.y + 1f)
                    p.gameObject.SetActive(false);
                else
                    p.gameObject.SetActive(true);
            }

            if (!stillFalling)
                allPiecesFall = true;
        }

        public override GridStateEnum CheckForNextState(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = (Grid)statedMono;

            if (allPiecesFall) //Yeah, one turn !
            {
                 return  GridStateEnum.WAITING_FOR_INPUT;
            }

            return this.stateType;
        }
    }
}
