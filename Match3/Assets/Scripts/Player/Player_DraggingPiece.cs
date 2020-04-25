
using BC_Solution;
using UnityEngine;

public partial class Player : StatedMono<PlayerStateEnum>
{

    public class DraggingPieceState : State
    {
        Grid currentGrid;
        Piece draggedPiece;

        public override PlayerStateEnum CheckForNextState(StatedMono<PlayerStateEnum> statedMono)
        {
            if (draggedPiece == null) //A click, you can replace this by a more sophisticated input module in real project.
            {
                return PlayerStateEnum.WAITING_FOR_INPUT;
            }
            return this.stateType;
        }

        public override void OnEnter(StatedMono<PlayerStateEnum> statedMono)
        {
            Player player = statedMono as Player;
            draggedPiece = player.frameDataBuffer.GetLast<MessageData<Piece>>(x => x.message == "PieceTaken").obj;
            currentGrid = ServiceProvider.GetService<Grid>(); //Just get the grid and catch it
        }

        public override void OnExit(StatedMono<PlayerStateEnum> statedMono)
        {
            if(draggedPiece)
                draggedPiece.Unselect();
        }

        public override void OnUpdate(StatedMono<PlayerStateEnum> statedMono)
        {
            Player player = statedMono as Player;
            Vector2 position = (Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z)));

            //Help the player to understand what is permitted
            position.x = Mathf.Clamp(position.x, draggedPiece.PhysicsPosition.x - 1, draggedPiece.PhysicsPosition.x + 1);
            position.y = Mathf.Clamp(position.y, -(draggedPiece.PhysicsPosition.y + 1), -(draggedPiece.PhysicsPosition.y - 1));

            //Not a real distance, but do the job
            float distToY = Mathf.Abs(position.y + draggedPiece.PhysicsPosition.y);
            float distToX = Mathf.Abs(position.x - draggedPiece.PhysicsPosition.x);


            //Goal: inside a small square where player can drag the piece freely, or outside the square : -> clamp to the lines
            //We can improve this with 1/x function, and find the nearest point to the curve, for lerping position between line clamping. But... no time for this, square + lerp do the job for this test.
            if(distToY > distToX && distToY > 0.2f) //Sort of clamp on a line (not true in case of rotation)
            {
                position.x = draggedPiece.PhysicsPosition.x;
            }
            else if (distToX > 0.2f)
                position.y = -draggedPiece.PhysicsPosition.y;
            //

            draggedPiece.ViewPosition = Vector2.Lerp(draggedPiece.ViewPosition, position, player.clock.DeltaRenderTime * 8f) ;

            if (!Input.GetMouseButton(0))
            {
                currentGrid.PlacePiece(draggedPiece, currentGrid.WorldToGridPosition( draggedPiece.ViewPosition));
                draggedPiece.Unselect();
                draggedPiece = null;
            }

        }
    }
}
