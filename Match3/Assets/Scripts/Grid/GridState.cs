
/// <summary>
/// Represent state of the grid
/// </summary>
public enum GridState
{
    
    NONE = 0, //No state
    SWITCHING_PIECES =1, //If the grid switch between 2 piece because player did a wrong move
    DELETING_PIECES = 2, //If the grid delete pieces because the player did a good move
    PIECE_BEING_DRAGGED = 4, //A piece is being dragged 
    UNLOCKING_SOFTLOCK = 8, //Softlock detected and the gris is changing some pieces to unlock the situation

}
