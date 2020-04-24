using BC_Solution;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represent the grid where piece are
/// </summary>
public partial class Grid : StatedMono<GridStateEnum>, IAwakable, IPositionProvider3D, IStartable
{
    /// <summary>
    /// Data structure to easily add Gems
    /// </summary>
    [System.Serializable]
    public struct PiecePool
    {
        public PieceTypeEnum pieceType;
        public ObjectPool objectPool;
    }

    [SerializeField]
    PiecePool[] piecePools = new PiecePool[0];


    [SerializeField]
    FrameDataBufferMono frameDataBuffer;

    [SerializeField]
    AbstractClockMono clock;

    [SerializeField]
    GridView gridView;

    private Vector2 size;
    public Vector2 Size
    {
        get => size;
    }

    /// <summary>
    /// Cached data for fast fetching piecePool by PieceType
    /// </summary>
    private Dictionary<PieceTypeEnum, PiecePool> piecePoolDictionary = new Dictionary<PieceTypeEnum, PiecePool>();


    /// <summary>
    /// Use this array to catch up where pieces are
    /// </summary>
    private Piece[][] gridPieces;

    /// <summary>
    /// PhysicsPosition and ViewPosition are the same in this applicaiton, for simplification.
    /// </summary>
    public Vector3 PhysicsPosition { get => this.transform.position; set => this.transform.position = value; }
    public Vector3 ViewPosition { get => this.transform.position; set => this.transform.position = value; }

    public void IAwake()
    {
        for (int i = 0; i < piecePools.Length; i++)
        {
            piecePoolDictionary.Add(piecePools[i].pieceType, piecePools[i]);
        }

        //On Separate file
        Add(GridStateEnum.WAITING_FOR_INPUT, new WaitingForInputState());
        Add(GridStateEnum.GENERATING_PIECES, new GeneratingPiecesState());
        Add(GridStateEnum.DELETING_PIECES, new DeletingPiecesState());
        Add(GridStateEnum.GENERATING_NEW_PIECES, new GeneratingNewPieces());
        Add(GridStateEnum.SWITCHING_PIECES, new SwitchingPiecesState());
        Add(GridStateEnum.PIECE_BEING_DRAGGED, new PieceBeingDraggedState());

        //
    }

    public void IStart()
    {
        StartBehaviour(GridStateEnum.WAITING_FOR_INPUT);
    }


    /// <summary>
    /// Generate a Random grid of pieces
    /// </summary>
    public void Generate(Vector2 size)
    {
        frameDataBuffer.AddData(new MessageData<Vector2>("Generate", size)); //I want to be frame and call order independant. so x message at the save frame are well handled in my FSM
    }

    /// <summary>
    /// Populate the grid array at index i, j with a piece
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    public Piece Populate(int i, int j)
    {
        if (gridPieces[i][j])
            gridPieces[i][j].GetComponent<PooledElement>().Pool();

        GameObject go = piecePools[Random.Range(0, piecePools.Length)].objectPool.GetFromPool();
        Piece p = go.GetComponent<Piece>();
        gridPieces[i][j] = p;
        p.gameObject.SetActive(false);
        p.PhysicsPosition = new Vector2(i, j);
        return p;
    }

    //Helper to place a piece in the grid
    public void InterChange(Piece p1, Piece p2)
    {
        gridPieces[(int)p1.PhysicsPosition.x][(int)p1.PhysicsPosition.y] = p2;                          //Just place them in the right place in the array. View position will be placed later.
        gridPieces[(int)p2.PhysicsPosition.x][(int)p2.PhysicsPosition.y] = p1;

        Vector2 temp = p1.PhysicsPosition;
        p1.PhysicsPosition = p2.PhysicsPosition;
        p2.PhysicsPosition = temp;
    }


    /// <summary>
    /// From bottom to top, and left to right, search the best combination
    /// </summary>
    /// <returns></returns>
    public List<Piece> GetFirstPiecesConnection(int minConnectionLength =3)
    {
       //We can cache this for performance, but I made the choice of clarity
        List<Piece> currentHorizontalConnection = new List<Piece>();
        List<Piece> currentVerticalConnection = new List<Piece>();
        List<Piece> currentBestConnection = null;

        for (int j = (int)size.y - 1; j >= 0; j--)
        {
            for (int i = 0; i < size.x; i++)
            {
                    GetRightConnectedPieces(i,j, ref currentHorizontalConnection); //Passing argument with ref to specify we modify the list
                    GetTopConnectedPieces(i, j, ref currentVerticalConnection); //Passing argument with ref to specify we modify the list

                    if (currentHorizontalConnection.Count > currentVerticalConnection.Count)
                    {
                        currentBestConnection = currentHorizontalConnection;
                    }
                    else
                    {
                        currentBestConnection = currentVerticalConnection;
                    }

                    if (currentBestConnection.Count >= minConnectionLength)
                    {
                        return currentBestConnection;
                    }
                }
        }

        return null;
    }

    /// <summary>
    /// Get horizontal connected piece by passing List<Piece> as ref
    /// </summary>
    /// <param name="pieces"></param>
    public void GetRightConnectedPieces(int i, int j,  ref List<Piece> pieces)
    {
        pieces.Clear();

        PieceTypeEnum firstPieceType = gridPieces[i][j].PieceType;
        pieces.Add(gridPieces[i][j]);

        for (int k = i+1; k < size.x; k++)  //We only need to check right side, because we begin algorithm at bottom left.
        {
            if (gridPieces[k][j].PieceType == firstPieceType)
            {
                pieces.Add(gridPieces[k][j]);
            }
            else return; //Break of connection
        }
    }

    /// <summary>
    /// Get vertical connected piece by passing List<Piece> as ref
    /// </summary>
    /// <param name="pieces"></param>
    public void GetTopConnectedPieces(int i, int j, ref List<Piece> pieces)
    {
        pieces.Clear();

        PieceTypeEnum firstPieceType = gridPieces[i][j].PieceType;
        pieces.Add(gridPieces[i][j]);

        for (int k = j - 1; k >= 0; k--)  //We only need to check top side, because we begin algorithm at bottom left.
        {
            if (gridPieces[i][k].PieceType == firstPieceType)
            {
                pieces.Add(gridPieces[i][k]);
            }
            else return; //Break of connection
        }
    }


    /// <summary>
    /// Ask the grid to have a piece, can return null if it doen't want to.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Piece AskForAPiece(Vector2 position)
    {
        position = WorldToGridPosition(position);

        if (CurrentStateType == GridStateEnum.WAITING_FOR_INPUT) //The only state where nothing is done, and all piece are in place
        {
            if(position.x >= 0 && position.x <= size.x && position.y >=0 && position.y <= size.y)
            {
                return this.gridPieces[(int)position.x][(int)position.y];
            }
        }

        return null;
    }

    public Vector2 WorldToGridPosition(Vector2 position)
    {
        position.y *= -1;
        position -= (Vector2)this.PhysicsPosition;
        position += new Vector2(0.5f, 0.5f);
        return position;
    }

    /// <summary>
    /// Assuming you will take a valid piece on the grid
    /// </summary>
    /// <param name="piece"></param>
    /// <returns></returns>
    public bool Take(Piece piece)
    {
        if (CurrentStateType == GridStateEnum.WAITING_FOR_INPUT) //The only state where nothing is done, and all piece are in place
        {
            frameDataBuffer.AddData(new MessageData<Piece>("PieceTaken", piece)); //Send this if you want to lock the grid or whatever

            return true;
        }

        return false;
    }

    /// <summary>
    /// Ask the grid to place a piece, (can be ommitted due to current grid state)
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="position"></param>
    public void PlacePiece(Piece piece, Vector2 position)
    {
        frameDataBuffer.AddData(new MessageData<Piece>("PlacePiece", piece)); //Send this and state will do the rest
    }

    /// <summary>
    /// Check if placing a PieceType at the position will occure in a valid connection
    /// </summary>
    /// <param name="pieceType"></param>
    /// <param name="placingPosition"></param>
    /// <param name="minConnectionLength"></param>
    /// <param name="bannedPosition">A position you want to ban (in case you swipe pieces)</param>
    /// <returns></returns>
    public bool PlacingWillCreateConnection(PieceTypeEnum pieceType, Vector2 placingPosition, Vector2 bannedPosition, int minConnectionLength = 3)
    {
        int x = (int)placingPosition.x;
        int y = (int)placingPosition.y;

        //Check Horizontal

        int cpt = 1; //The first piece we place
        for (int i = (int)placingPosition.x -1; i >= 0 && ( i < bannedPosition.x || bannedPosition.y != y); i--)
        {
            if (this.gridPieces[i][y].PieceType == pieceType)
            {
                cpt++;
                if (cpt >= minConnectionLength)
                    return true;
            }
            else break;
        }

        //cpt = 1; //The first piece we place  --> oops
        for (int i = (int)placingPosition.x + 1; i < size.x && (i > bannedPosition.x || bannedPosition.y != y); i++)
        {
            if (this.gridPieces[i][y].PieceType == pieceType)
            {
                cpt++;
                if (cpt >= minConnectionLength)
                    return true;
            }
            else break;
        }

        //Check Vertical

        cpt = 1; //The first piece we place
        for (int i = (int)placingPosition.y - 1; i >= 0 && ( i < bannedPosition.y || bannedPosition.x != x); i--)
        {
            if (this.gridPieces[x][i].PieceType == pieceType)
            {
                cpt++;
                if (cpt >= minConnectionLength)
                    return true;
            }
            else break;
        }

        //cpt = 1; //The first piece we place  --> oops
        for (int i = (int)placingPosition.y + 1; i < size.y && (i > bannedPosition.y || bannedPosition.x != x); i++)
        {
            if (this.gridPieces[x][i].PieceType == pieceType)
            {
                cpt++;
                if (cpt >= minConnectionLength)
                    return true;
            }
            else break;
        }

        return false; //What a bad player (^^)
    }
}
