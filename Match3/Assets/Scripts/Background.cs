using UnityEngine;

public class Background : MonoBehaviour
{

    [SerializeField]
    Animator animator;


    public void Highlight(bool b)
    {
        animator.SetBool("highlight", b);
    }

    public void Select(bool b)
    {
        animator.SetBool("selected", b);
    }

    public void Hover(bool b)
    {
        animator.SetBool("hover", b);
    }

    public void Synchronize(float time)
    {
        animator.SetFloat("normalizedTime",  time/animator.GetCurrentAnimatorStateInfo(0).length);
    }

    public bool IsInHighlightState()
    {
       return animator.GetCurrentAnimatorStateInfo(0).IsName("Highlight");
    }
}
