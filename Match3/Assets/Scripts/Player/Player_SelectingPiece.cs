
using BC_Solution;
using UnityEngine;

public partial class Player : StatedMono<PlayerStateEnum>
{
    /// <summary>
    /// Nearly the same as waiting input, with small differences
    /// </summary>
    public class SelectingPieceState : State
    {
        Grid currentGrid;
        Piece foundPiece;
        Piece selectedPiece;

        public override PlayerStateEnum CheckForNextState(StatedMono<PlayerStateEnum> statedMono)
        {
            Player player = statedMono as Player;
            if (player.frameDataBuffer.Exists<MessageData<Piece>>((x)=>x.message == "PieceTaken"))
            {
                return PlayerStateEnum.DRAGGING_PIECE;
            }

            if (foundPiece)
                return PlayerStateEnum.WAITING_FOR_INPUT;

            return this.stateType;
        }

        public override void OnEnter(StatedMono<PlayerStateEnum> statedMono)
        {
            Player player = statedMono as Player;
            currentGrid = ServiceProvider.GetService<Grid>(); //Just get the grid and catch it
            selectedPiece = player.frameDataBuffer.GetLast<MessageData<Piece>>(x => x.message == "PieceSelected").obj;
            foundPiece = null;
        }

        public override void OnExit(StatedMono<PlayerStateEnum> statedMono)
        {
            Player player = statedMono as Player;
            if (selectedPiece && !player.frameDataBuffer.Exists<MessageData<Piece>>((x) => x.message == "PieceTaken"))
                selectedPiece.Unselect();
        }

        public override void OnUpdate(StatedMono<PlayerStateEnum> statedMono)
        {
            Player player = statedMono as Player;

            if (Input.GetMouseButton(0)) //Drag the piece if player clic again on the same piece
            {
                Vector2 position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
                if(Vector2.SqrMagnitude((Vector2)selectedPiece.ViewPosition - position) < 0.5f)
                {
                    player.frameDataBuffer.AddData(new MessageData<Piece>("PieceTaken", selectedPiece)); //Pass the data to the next state
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector2 position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
                foundPiece = currentGrid.AskForAPiece(position);

                if(foundPiece)
                {
                    currentGrid.PlacePiece(selectedPiece, foundPiece.PhysicsPosition);
                }
            }
        }
    }
}
