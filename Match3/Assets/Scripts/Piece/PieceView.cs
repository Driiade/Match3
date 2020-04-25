using BC_Solution;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The view of the Piece
/// Handle effect (image and sound)
/// </summary>
public class PieceView : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;

    [SerializeField]
    AudioClip audioDestroyClip;

    public int sortingOrderWhenSelected = 2;

    public int sortingOrderWhenNotSelected = 1;

    public void Select()
    {
        spriteRenderer.sortingOrder = sortingOrderWhenSelected;
    }

    public void UnSelect()
    {
        spriteRenderer.sortingOrder = sortingOrderWhenNotSelected;
    }

    public void Destroy()
    {

    }

}
