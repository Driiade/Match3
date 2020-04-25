using BC_Solution;
using System.Collections.Generic;
using UnityEngine;

public partial class Grid
{
    public class SwitchingPiecesState : State
    {
        float startTime;

        Piece p1;
        Piece p2;

        Vector2 p1StartPosition;
        Vector2 p2StartPosition;

        Vector2 p1EndPosition;
        Vector2 p2EndPosition;

        private float timeToSwitch = 0.2f;
        private List<Piece> playerConnections;

        public override GridStateEnum CheckForNextState(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = statedMono as Grid;
            float deltaTime = grid.clock.CurrentRenderTime - startTime;

            if (deltaTime > timeToSwitch)
            {
                if (playerConnections != null)
                {
                    return GridStateEnum.DELETING_PIECES;
                }
                else
                    return GridStateEnum.WAITING_FOR_INPUT;
            }

            return this.stateType;
        }

        /// <summary>
        /// I really don't check if p1 == p2, it's unecessary. The only edge case is in doubling movement speed (And I don't care about this for the moment)
        /// </summary>
        /// <param name="statedMono"></param>
        public override void OnEnter(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = statedMono as Grid;
            p1 = grid.frameDataBuffer.GetLast<MessageData<Piece>>((x)=>x.message == "PlacePiece").obj;
            Vector2 position = grid.WorldToGridPosition(p1.ViewPosition);

            p2 = grid.gridPieces[(int)position.x][(int)position.y];

            startTime = grid.clock.CurrentRenderTime;

            p1StartPosition = p1.ViewPosition;
            p2StartPosition = p2.ViewPosition;

            if(grid.PlacingWillCreateConnection(p1.PieceType, p2.PhysicsPosition, p1.PhysicsPosition) || grid.PlacingWillCreateConnection(p2.PieceType, p1.PhysicsPosition, p2.PhysicsPosition))
            {
                p1EndPosition = p2.PhysicsPosition;
                p2EndPosition = p1.PhysicsPosition;
                grid.InterChange(p1, p2);

                playerConnections = grid.GetCrossConnections(p1.PieceType, p1.PhysicsPosition);

                ServiceProvider.GetService<SFXSystem>().PlaySFX(grid.validSwapAudioClip);
            }
            else
            {
                p1EndPosition = p1.PhysicsPosition;
                p2EndPosition = p2.PhysicsPosition;
                playerConnections = null;

                ServiceProvider.GetService<SFXSystem>().PlaySFX(grid.badSwapAudioClip);
            }
        }

        public override void OnExit(StatedMono<GridStateEnum> statedMono)
        {
            //Ensure we are well placed
            p1.ViewPosition = new Vector2(p1EndPosition.x, -p1EndPosition.y);
            p2.ViewPosition = new Vector2(p2EndPosition.x, -p2EndPosition.y);
        }



        /// <summary>
        /// Main purpose: Add a small effect when generating piece
        /// </summary>
        /// <param name="statedMono"></param>
        public override void OnUpdate(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = statedMono as Grid;
            float deltaTime = grid.clock.CurrentRenderTime - startTime;
            //Ok do the switch

            float speed = 1f/timeToSwitch; //We can catch this for performance
            p1.ViewPosition = Vector2.Lerp(p1StartPosition, new Vector2(p1EndPosition.x, -p1EndPosition.y), deltaTime * speed);
            p2.ViewPosition = Vector2.Lerp(p2StartPosition, new Vector2(p2EndPosition.x, -p2EndPosition.y), deltaTime * speed);

            if (playerConnections != null)
            {
                grid.frameDataBuffer.AddData(new MessageData<List<Piece>>( "DeletePieces", playerConnections)); //This is not very CPU friendly, but I lack a sort of "blackboard" tool 
            }
        }

    }
}
