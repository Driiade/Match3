using BC_Solution;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The view of a grid
/// </summary>
public class GridView : MonoBehaviour
{
    [SerializeField]
    Grid grid;

    [SerializeField]
    float lineWidth = 0.2f;

    public float zGridOffset;

    [Space(10)]
    [Header("Framing")]
    [SerializeField]
    Transform framing;

    public Vector2 scaleFactor = Vector2.one;

    [Space(10)]
    [Header("Mask")]
    [SerializeField]
    Transform mask;

    [SerializeField]
    ObjectPool piecebackGroundObjectPool;

    [SerializeField]
    Material lineMaterial;

    private Vector2 size;

    /// <summary>
    /// Use this array to catch up where pieces are
    /// </summary>
    private Background[][] gridBackgrounds;

    private float highlightTime =0;

    public void Initialize(Vector2 size)
    {
        this.size = size;
        gridBackgrounds = ArrayExtension.New2DArray<Background>((int)size.x, (int)size.y);

        piecebackGroundObjectPool.PoolAll();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {

                GameObject go = piecebackGroundObjectPool.GetFromPool();
                go.transform.position = new Vector2(i, -j);

                gridBackgrounds[i][j] = go.GetComponent<Background>();
            }
        }
    }


    private void Update()
    {
        framing.localScale = scaleFactor * grid.Size;

        Vector2 gridCenter = new Vector2(0.5f * grid.Size.x - 0.5f * ((grid.Size.x + 1) % 2), -0.5f * grid.Size.y + 0.5f * ((grid.Size.y + 1) % 2));
        framing.localPosition = gridCenter;

        mask.localScale = grid.Size;
        mask.localPosition = gridCenter;


        for (int i = 0; i < size.x + 1; ++i)
        {
            for (int j = 0; j < size.y; j++)
            {
                if(gridBackgrounds[i][j].IsInHighlightState())
                {
                    gridBackgrounds[i][j].Synchronize(Time.time - highlightTime);
                }
            }
        }
    }



    //Draw a GL Grid
    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);

        GL.Begin(GL.QUADS);

        GL.Color(new Color(0, 0, 0, 1));

        Vector3 offset = new Vector3(-0.5f * ((grid.Size.x + 1) % 2) - lineWidth, 0.5f + 0.5f * ((grid.Size.y + 1) % 2), zGridOffset); //Center on the grid. We use complicate algorithm to handle all cases, but we don't really need that.

        //Horizontal
        for (int i = 0; i < grid.Size.x; ++i)
        {
            for (int j = 0; j < grid.Size.y +1; j++)
            {
                float yT = -j - 0.5f + lineWidth;
                float yB = -j - 0.5f - lineWidth;

                //Draw Line
                GL.Vertex(new Vector3(0, yT, 0) + offset);
                GL.Vertex(new Vector3(grid.Size.x + 2f * lineWidth, yT, 0) + offset);

                GL.Vertex(new Vector3(grid.Size.x + 2f * lineWidth, yB, 0) + offset);
                GL.Vertex(new Vector3(0, yB, 0) + offset);
            }
        }

        offset = new Vector3(-0.5f + 0.5f * ((grid.Size.x + 1) % 2), 0.5f * ((grid.Size.y + 1) % 2) + lineWidth, zGridOffset); //Center on the grid. We use complicate algorithm to handle all cases, but we don't really need that.
        //Vertical
        for (int i = 0; i < grid.Size.x +1; ++i)
        {
            for (int j = 0; j < grid.Size.y; j++)
            {
                float xR = i - 0.5f + lineWidth;
                float xL = i - 0.5f - lineWidth;

                //Draw Line
                GL.Vertex(new Vector3(xL, 0, 0) + offset);
                GL.Vertex(new Vector3(xR, 0, 0) + offset);

                GL.Vertex(new Vector3(xR, -grid.Size.y - 2f * lineWidth, 0) + offset);
                GL.Vertex(new Vector3(xL, -grid.Size.y - 2f*lineWidth, 0) + offset);
            }
        }

        GL.End();
        GL.PopMatrix();
    }

    public void StartHighlightTime()
    {
        highlightTime = Time.time;
    }


    public void StartHighlighting(List<Vector2> positions)
    {
        foreach (Vector2 position in positions)
        {
            gridBackgrounds[(int)position.x][(int)position.y].Highlight(true);         
        }
    }

    public void Select(Vector2 position, bool b)
    {
        gridBackgrounds[(int)position.x][(int)position.y].Select(b);
    }

    public void Hover(Vector2 position, bool b)
    {
        gridBackgrounds[(int)position.x][(int)position.y].Hover(b);
    }

    public void StopHighlighting(List<Vector2> positions)
    {
        foreach (Vector2 position in positions)
        {
            gridBackgrounds[(int)position.x][(int)position.y].Highlight(false);        
        }
    }
}
