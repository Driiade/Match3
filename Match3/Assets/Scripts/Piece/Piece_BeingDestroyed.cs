using BC_Solution;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Piece : StatedMono<PieceStateEnum>
{

    public class BeingDestroyedState : State
    {
        float timerForPooling;

        public override PieceStateEnum CheckForNextState(StatedMono<PieceStateEnum> statedMono)
        {
            Piece piece = statedMono as Piece;
            if (timerForPooling <= piece.clock.CurrentRenderTime)
                return PieceStateEnum.WAITING_FOR_INPUT;
            else
                return this.stateType;
        }

        public override void OnEnter(StatedMono<PieceStateEnum> statedMono)
        {
            Piece piece = statedMono as Piece;
            ServiceProvider.GetService<SFXSystem>().PlayUniqueSFX(piece.destroyAudioClip);
            timerForPooling = piece.clock.CurrentRenderTime + 0.2f;
        }

        public override void OnExit(StatedMono<PieceStateEnum> statedMono)
        {
            Piece piece = statedMono as Piece;
            piece.pooledElement.Pool();
        }

        public override void OnUpdate(StatedMono<PieceStateEnum> statedMono)
        {

        }
    }
}
