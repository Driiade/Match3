using UnityEngine;

public class Background : MonoBehaviour
{

    [SerializeField]
    Animator animator;


    public void Highlight(bool b)
    {
        animator.SetBool("highlight", b);
    }

}
