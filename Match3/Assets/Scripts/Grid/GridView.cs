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

    [Space(10)]
    [Header("Framing")]
    [SerializeField]
    Transform framing;

    public Vector2 scaleFactor = Vector2.one;

    [Space(10)]
    [Header("Mask")]
    [SerializeField]
    Transform mask;

    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        // Unity has a built-in shader that is useful for drawing
        // simple colored things.
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        lineMaterial = new Material(shader);
        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        // Turn on alpha blending
        lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        // Turn backface culling off
        lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        // Turn off depth writes
        lineMaterial.SetInt("_ZWrite", 0);
    }

    void Awake()
    {
        if (!lineMaterial)
        {
            CreateLineMaterial();
        }
    }

    private void Update()
    {
        framing.localScale = scaleFactor * grid.Size;

        Vector2 gridCenter = new Vector2(0.5f * grid.Size.x - 0.5f * ((grid.Size.x + 1) % 2), -0.5f * grid.Size.y + 0.5f * ((grid.Size.y + 1) % 2));
        framing.localPosition = gridCenter;

        mask.localScale = grid.Size;
        mask.localPosition = gridCenter;
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

        Vector3 offset = new Vector3(-0.5f * ((grid.Size.x + 1) % 2) - lineWidth, 0.5f + 0.5f * ((grid.Size.y + 1) % 2), 0); //Center on the grid. We use complicate algorithm to handle all cases, but we don't really need that.

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

        offset = new Vector3(-0.5f + 0.5f * ((grid.Size.x + 1) % 2), 0.5f * ((grid.Size.y + 1) % 2) + lineWidth, 0); //Center on the grid. We use complicate algorithm to handle all cases, but we don't really need that.
        //Vertical
        for (int i = 0; i < grid.Size.x +1; ++i)
        {
            for (int j = 0; j < grid.Size.y; j++)
            {
                float xR = i - 0.5f + lineWidth;
                float xL = i - 0.5f - lineWidth;

                //Draw Line
                GL.Vertex(new Vector3(xR, 0, 0) + offset);
                GL.Vertex(new Vector3(xL, 0, 0) + offset);

                GL.Vertex(new Vector3(xL, -grid.Size.y - 2f*lineWidth, 0) + offset);
                GL.Vertex(new Vector3(xR, -grid.Size.y - 2f * lineWidth, 0) + offset);
            }
        }

        GL.End();
        GL.PopMatrix();
    }


}
