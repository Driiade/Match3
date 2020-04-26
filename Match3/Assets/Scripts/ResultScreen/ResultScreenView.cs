using UnityEngine;
using TMPro;
using BC_Solution;

public class ResultScreenView : MonoBehaviour
{
    [SerializeField]
    Animator animator;

    [SerializeField]
    TextMeshProUGUI scoreText;

    [SerializeField]
    TextMeshProUGUI bextScoreText;

    public void Show(int score, int bestScore)
    {
        animator.SetBool("show", true);
        scoreText.text = score.ToString();

        bextScoreText.text = bestScore.ToString();
    }

}
