using BC_Solution;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represent the grid where piece are
/// </summary>
public class Grid : StatedMono<GridStateEnum>, IAwakable, IPositionProvider3D
{

    public class GeneratingPiecesState : State
    {
        float startTime;

        public override void OnEnter(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = (Grid)statedMono;
            Vector2 size = grid.frameDataBuffer.GetLast<MessageData<Vector2>>((x) => x.message == "Generate").obj;
            grid.size = size;
            grid.gridPieces = ArrayExtension.New2DArray<Piece>((int)size.x, (int)size.y);

            for (int i = 0; i < grid.piecePools.Length; i++)
            {
                grid.piecePools[i].objectPool.PoolAll();
            }

            for (int i = 0; i < grid.size.x; i++)
            {
                for (int j = 0; j < grid.size.y; j++)
                {
                    GameObject go = grid.piecePools[Random.Range(0, grid.piecePools.Length)].objectPool.GetFromPool();
                    grid.gridPieces[i][j] = go.GetComponent<Piece>();
                    grid.gridPieces[i][j].gameObject.SetActive(false);
                }
            }

            startTime = grid.clock.CurrentRenderTime;
        }

        public override void OnExit(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = (Grid)statedMono;
            for (int i = 0; i < grid.size.x; i++)
            {
                for (int j = 0; j < grid.size.y; j++)
                {
                    grid.gridPieces[i][j].PhysicsPosition = new Vector3(i, -j, grid.PhysicsPosition.z); //Starting to left top
                }
            }
        }

        /// <summary>
        /// Main purpose: Add a small effect when generating piece
        /// </summary>
        /// <param name="statedMono"></param>
        public override void OnUpdate(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = (Grid)statedMono;
            float deltaTime = grid.clock.CurrentRenderTime - startTime;

            for (int i = 0; i < grid.size.x; i++)
            {
                for (int j = 0; j < grid.size.y; j++)
                {
                    grid.gridPieces[i][j].PhysicsPosition = new Vector3(i, Mathf.Max(-j, -j + 6f *Mathf.Sin(i*0.2f) - deltaTime * 10f + grid.size.y), grid.PhysicsPosition.z); //Starting to left top

                    if (grid.gridPieces[i][j].PhysicsPosition.y > grid.PhysicsPosition.y + 0.5f)
                        grid.gridPieces[i][j].gameObject.SetActive(false);
                    else
                        grid.gridPieces[i][j].gameObject.SetActive(true);
                }
            }

            if (deltaTime > 2f)
                grid.SwitchTo(GridStateEnum.WAITING_FOR_INPUT);
        }
    }

    public class WaitingForInputState : State
    {
        public override void OnEnter(StatedMono<GridStateEnum> statedMono)
        {
            //
        }

        public override void OnExit(StatedMono<GridStateEnum> statedMono)
        {
          //
        }

        public override void OnUpdate(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = statedMono as Grid;
            List<Piece> connections = grid.GetFirstPiecesConnection(3);
            if (connections != null)
            {
                grid.frameDataBuffer.AddData(new MessageData<List<Piece>>("DeletePieces", connections));
            }


            //Gestion state by message
            if(grid.frameDataBuffer.Exists<MessageData<Vector2>>((x)=>x.message == "Generate" ))
            {
                grid.SwitchTo(GridStateEnum.GENERATING_PIECES);
                return;
            }

            if(grid.frameDataBuffer.Exists<MessageData<List<Piece>>>((x) => x.message == "DeletePieces"))
            {
                grid.SwitchTo(GridStateEnum.DELETING_PIECES);
                return;
            }
        }
    }


    public class DeletingPiecesState : State
    {
        private List<Piece> pieceToDestroy;

        private float timer;
        public override void OnEnter(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = (Grid)statedMono;
            pieceToDestroy = grid.frameDataBuffer.GetLast<MessageData<List<Piece>>>((x) => x.message == "DeletePieces").obj;
            timer = grid.clock.CurrentRenderTime + 2f;
        }

        public override void OnExit(StatedMono<GridStateEnum> statedMono)
        {

        }

        public override void OnUpdate(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = (Grid)statedMono;
            if (grid.clock.CurrentRenderTime> timer)
            {
                for (int i = 0; i < pieceToDestroy.Count; i++)
                {
                    pieceToDestroy[i].GetComponent<PooledElement>().Pool();
                }
            }
        }
    }


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

        Add(GridStateEnum.WAITING_FOR_INPUT, new WaitingForInputState());
        Add(GridStateEnum.GENERATING_PIECES, new GeneratingPiecesState());
        Add(GridStateEnum.DELETING_PIECES, new DeletingPiecesState());

        SwitchTo(GridStateEnum.WAITING_FOR_INPUT);
    }

    /// <summary>
    /// Generate a Random grid of pieces
    /// </summary>
    public void Generate(Vector2 size)
    {
        frameDataBuffer.AddData(new MessageData<Vector2>("Generate", size)); //I want to be frame and call order independant. so x message at the save frame are well handled in my FSM

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

        for (int i = 0; i < size.x; i++)
        {
            for (int j = (int)size.y-1; j >=0; j--)
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
                    Debug.Log(currentBestConnection.Count);
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
}
