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
                    grid.Populate(i, j);
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
                    grid.gridPieces[i][j].ViewPosition = new Vector3(i, -j, grid.ViewPosition.z); //Starting to left top
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

            if (deltaTime > 2f)
            {
                grid.SwitchTo(GridStateEnum.WAITING_FOR_INPUT);
                return;
            }

            for (int i = 0; i < grid.size.x; i++)
            {
                for (int j = 0; j < grid.size.y; j++)
                {
                    grid.gridPieces[i][j].ViewPosition = new Vector3(i, Mathf.Max(-j, -j + 6f *Mathf.Sin(i*0.2f) - deltaTime * 10f + grid.size.y), grid.ViewPosition.z); //Starting to left top

                    if (grid.gridPieces[i][j].ViewPosition.y > grid.ViewPosition.y + 0.5f)
                        grid.gridPieces[i][j].gameObject.SetActive(false);
                    else
                        grid.gridPieces[i][j].gameObject.SetActive(true);
                }
            }
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

            //Gestion state by message
            if (grid.frameDataBuffer.Exists<MessageData<Vector2>>((x) => x.message == "Generate"))
            {
                grid.SwitchTo(GridStateEnum.GENERATING_PIECES);
                return;
            }

            if (grid.frameDataBuffer.Exists<MessageData<List<Piece>>>((x) => x.message == "DeletePieces"))
            {
                grid.SwitchTo(GridStateEnum.DELETING_PIECES);
                return;
            }

            List<Piece> connections = grid.GetFirstPiecesConnection(3);
            if (connections != null)
            {
                grid.frameDataBuffer.AddData(new MessageData<List<Piece>>("DeletePieces", connections));
                return; //no player input authorized
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
            timer = grid.clock.CurrentRenderTime + 0.5f;
        }

        public override void OnExit(StatedMono<GridStateEnum> statedMono)
        {

        }

        public override void OnUpdate(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = (Grid)statedMono;

            if (grid.frameDataBuffer.Exists<MessageData<List<Vector2>>>((x) => x.message == "GeneratePieces"))
            {
                grid.SwitchTo(GridStateEnum.GENERATING_NEW_PIECES);
                return;
            }


            if (pieceToDestroy.Count > 0)
            {
                if (grid.clock.CurrentRenderTime > timer)
                {
                    List<Vector2> positions = new List<Vector2>(pieceToDestroy.Count);
                    for (int i = 0; i < pieceToDestroy.Count; i++)
                    {
                        pieceToDestroy[i].GetComponent<PooledElement>().Pool();
                        positions.Add(pieceToDestroy[i].PhysicsPosition);
                    }

                    pieceToDestroy.Clear();

                    grid.frameDataBuffer.AddData(new MessageData<List<Vector2>>("GeneratePieces", positions));
                }
            }

        }
    }

    public class GeneratingNewPieces : State
    {
        List<Vector2> missingPosition;
        List<Piece> piecesToMove = new List<Piece>();

        float yVelocity;
        bool allPiecesFall = false;

        public float gravity = -9.18f;

        public override void OnEnter(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = (Grid)statedMono;
            allPiecesFall = false;
            missingPosition = grid.frameDataBuffer.GetLast<MessageData<List<Vector2>>>((x) => x.message == "GeneratePieces").obj;
            piecesToMove.Clear();

            //Ok now count what is missing per column
            int[] missingPiecePerColumn = new int[(int)grid.size.x];
            int[] maxYPerColumn = new int[(int)grid.size.x];

            for (int i = 0; i < missingPosition.Count; i++)
            {
                int x = (int)missingPosition[i].x;
                missingPiecePerColumn[x]+=1;
                maxYPerColumn[x] = Mathf.Max(maxYPerColumn[x], (int)missingPosition[i].y);
            }



            for (int i = 0; i < missingPiecePerColumn.Length; i++)  //Just place them in the right place in the array. View position will be placed later.
            {
                int maxY = maxYPerColumn[i];
                for (int k = 0; k < missingPiecePerColumn[i]; k++)
                {
                    for (int j = maxY; j >= 1; j--)
                    {
                        grid.InterChange(grid.gridPieces[i][j - 1], grid.gridPieces[i][j]);
                    }

                    Piece p = grid.Populate(i, 0);
                    p.ViewPosition = new Vector2(i, (k + 1));
                }

                for (int j = 0; j < maxYPerColumn[i]+1; j++)
                {
                    piecesToMove.Add(grid.gridPieces[i][j]);
                }
            }
          
            yVelocity = 0f;
        }

        public override void OnExit(StatedMono<GridStateEnum> statedMono)
        {
            //Just replace all pieces, jsut in case
            Grid grid = (Grid)statedMono;
            foreach (var item in piecesToMove)
            {
                item.ViewPosition = new Vector2(item.PhysicsPosition.x, -item.PhysicsPosition.y);
                item.gameObject.SetActive(true);
            }
        }


        public override void OnUpdate(StatedMono<GridStateEnum> statedMono)
        {
            Grid grid = (Grid)statedMono;

            if (allPiecesFall) //Yeah, one turn !
            {
                grid.SwitchTo(GridStateEnum.WAITING_FOR_INPUT);
                return;
            }


            float deltaTime = grid.clock.DeltaRenderTime;
            float gt = gravity * deltaTime;
            yVelocity += gt;
            gt *= 0.5f*deltaTime;

            float deltaPosition = yVelocity * deltaTime + gt; //Cache this

            bool stillFalling = false;
            foreach (var p in piecesToMove)
            {
                Vector2 targetPosition = p.PhysicsPosition;
                targetPosition.y *= -1f;

                p.ViewPosition = new Vector2(p.ViewPosition.x, Mathf.Max(targetPosition.y, p.ViewPosition.y + deltaPosition));

                if (!stillFalling && p.ViewPosition.y - targetPosition.y >= 0.01f)
                    stillFalling = true;

                if (p.ViewPosition.y > grid.ViewPosition.y + 0.5f)
                    p.gameObject.SetActive(false);
                else
                    p.gameObject.SetActive(true);
            }

            if (!stillFalling)
                allPiecesFall = true;
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
        Add(GridStateEnum.GENERATING_NEW_PIECES, new GeneratingNewPieces());

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
}
