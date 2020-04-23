using BC_Solution;
using UnityEngine;

/// <summary>
/// Script which focus a camera to a grid
/// </summary>
public class GridCamera : MonoBehaviour
{
    [SerializeField]
    Vector3 offset;

    void LateUpdate()
    {
        Grid grid = ServiceProvider.GetService<Grid>();
        Vector3 newPosition = grid.ViewPosition + (Vector3)(new Vector2(grid.Size.x, -grid.Size.y) / 2f);
        newPosition -= new Vector3(-0.5f * ((grid.Size.x+1)%2), -0.5f * ((grid.Size.y+1) % 2), 0); //Center on the grid. We use complicate algorithm to handle all cases, but we don't really need that.
        newPosition += offset;
        this.transform.position = newPosition;
    }

}
