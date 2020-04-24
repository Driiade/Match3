
using BC_Solution;
using UnityEngine;

public partial class Player : StatedMono<PlayerStateEnum>
{

    public class WaitingForInputState : State
    {
        Grid currentGrid;
        Piece foundPiece;

        public override PlayerStateEnum CheckForNextState(StatedMono<PlayerStateEnum> statedMono)
        {
            if (foundPiece)
                return PlayerStateEnum.DRAGGING_PIECE;

            return this.stateType;
        }

        public override void OnEnter(StatedMono<PlayerStateEnum> statedMono)
        {
            foundPiece = null;
            currentGrid = ServiceProvider.GetService<Grid>(); //Just get the grid and catch it
        }

        public override void OnExit(StatedMono<PlayerStateEnum> statedMono)
        {

        }

        public override void OnUpdate(StatedMono<PlayerStateEnum> statedMono)
        {
            if (Input.GetMouseButton(0)) //A click, you can replace this by a more sophisticated input module in real project.
            {
                foundPiece = currentGrid.AskForAPiece(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z)));
                if (foundPiece)
                {
                    if (currentGrid.Take(foundPiece)) //Just in case, double check we really can get this
                    {
                        Player player = statedMono as Player;
                        player.frameDataBuffer.AddData(new MessageData<Piece>("PieceTaken", foundPiece)); //Pass the data to the next state
                        foundPiece.BeTaken();
                    }
                }
            }
        }
    }

}
