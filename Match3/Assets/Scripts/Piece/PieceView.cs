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
    ParticleSystem destroyParticleSystem;

    [SerializeField]
    Animator animator;

    public int sortingOrderWhenSelected = 2;

    public int sortingOrderWhenNotSelected = 1;

     void OnEnable()
    {
        spriteRenderer.sortingOrder = sortingOrderWhenNotSelected;
        spriteRenderer.gameObject.SetActive(true);
        destroyParticleSystem.Stop();
    }

    public void Select()
    {
        spriteRenderer.sortingOrder = sortingOrderWhenSelected;
        animator.SetBool("selected", true);
    }

    public void UnSelect()
    {
        spriteRenderer.sortingOrder = sortingOrderWhenNotSelected;
        animator.SetBool("selected", false);
    }

    public void PlayDestroyVFX()
    {
        destroyParticleSystem.Play();
        spriteRenderer.gameObject.SetActive(false);
    }

}
