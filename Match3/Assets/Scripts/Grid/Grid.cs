using BC_Solution;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represent the grid where piece are
/// </summary>
public class Grid : StatedMono<GridState>, IAwakable, IPositionProvider3D
{
    /// <summary>
    /// Data structure to easily add Gems
    /// </summary>
    [System.Serializable]
    public struct PiecePool
    {
        public PieceType pieceType;
        public ObjectPool objectPool;
    }

    [SerializeField]
    Vector2 size;

    [SerializeField]
    PiecePool[] piecePools = new PiecePool[0];

    public Vector2 Size
    {
        get => size;
    }

    /// <summary>
    /// Cached data forfast fetching piecePool by PieceType
    /// </summary>
    private Dictionary<PieceType, PiecePool> piecePoolDictionary = new Dictionary<PieceType, PiecePool>();

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
    }

    void Awake()
    {
        IAwake();
        Generate();
    }

    /// <summary>
    /// Generate a Random grid of pieces
    /// </summary>
    public void Generate()
    {
        for (int i = 0; i < piecePools.Length; i++)
        {
            piecePools[i].objectPool.PoolAll();
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                GameObject go = piecePools[Random.Range(0, piecePools.Length)].objectPool.GetFromPool();
                IPositionProvider3D positionProvider =  go.GetComponent<IPositionProvider3D>();
                positionProvider.PhysicsPosition = new Vector3(i, -j, this.PhysicsPosition.z); //Starting to left top
            }
        }
    }
}
