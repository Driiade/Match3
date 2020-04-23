

/// <summary>
/// Represent state of the pieces
/// </summary>
public enum PieceStateEnum
{
   NONE=0, //no state
   DRAGGED=1, //Something drag the piece
   SWITCHING=2, //Currently switching with an other
   BEING_DESTROYED =4, //Currently destroyed
   APPEARING =8, //Appearing on the grid
   CHANGING_TYPE = 16, //Changing type after softlock
   WAITING_FOR_INPUT = 32,
}
