
using BC_Solution;
using UnityEngine;

public partial class Player : StatedMono<PlayerStateEnum>
{

    public class WaitingForInputState : State
    {
        Grid currentGrid;
        Piece foundPiece;

        Vector2? startDragPoint;
        PlayerStateEnum nextState;

        public override PlayerStateEnum CheckForNextState(StatedMono<PlayerStateEnum> statedMono)
        {
            if (foundPiece)
                return nextState;

            return this.stateType;
        }

        public override void OnEnter(StatedMono<PlayerStateEnum> statedMono)
        {
            foundPiece = null;
            currentGrid = ServiceProvider.GetService<Grid>(); //Just get the grid and catch it
            startDragPoint = null;
        }

        public override void OnExit(StatedMono<PlayerStateEnum> statedMono)
        {

        }

        public override void OnUpdate(StatedMono<PlayerStateEnum> statedMono)
        {
            string message =null;
            if (Input.GetMouseButton(0)) //A click, you can replace this by a more sophisticated input module in real project.
            {
                //This is algorithm for testing if player did 1% of the distance of the diagonal of it screen.

                //if (startDragPoint == null)
                //    startDragPoint = Input.mousePosition;

                //if (Vector2.Distance((Vector2)startDragPoint, Input.mousePosition) /(Mathf.Sqrt(Screen.height* Screen.height + Screen.width* Screen.width)) >= 0.01)
                //

                if (startDragPoint == null)
                    startDragPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));


                //But I prefere to check if player did 10% of the current grid cellul as a swipe
                if (Vector2.Distance((Vector2)startDragPoint, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z))) >= 0.1)
                {
                    foundPiece = currentGrid.AskForAPiece((Vector2)startDragPoint);
                    message = "PieceTaken";
                    nextState = PlayerStateEnum.DRAGGING_PIECE;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Vector2 position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
                foundPiece = currentGrid.AskForAPiece(position);
                message = "PieceSelected";
                nextState = PlayerStateEnum.SELECTING_PIECE;
            }
            else
            {
                startDragPoint = null;
            }

            if (foundPiece)
            {
                currentGrid.Take(foundPiece); //Just in case, double check we really can get this
                Player player = statedMono as Player;
                player.frameDataBuffer.AddData(new MessageData<Piece>(message, foundPiece)); //Pass the data to the next state
                foundPiece.Select();
            }
        }
    }

}
