using UnityEngine;
using TMPro;
using BC_Solution;

/// <summary>
/// View for the timer
/// </summary>
public class ScoreView : MonoBehaviour, IAwakable, IStartable
{
    [SerializeField]
    TextMeshProUGUI text;


    private ScoreSystem scoreSystem;

    public void IAwake()
    {
        text.text = "0";
    }

    public void IStart()
    {
        scoreSystem = ServiceProvider.GetService<ScoreSystem>();
        scoreSystem.OnAddScore += UpdateView;
    }

    void OnDestroy()
    {
        scoreSystem.OnAddScore -= UpdateView;
    }

    // Update is called once per frame
    void UpdateView(int score)
    {
        text.text = score.ToString();
    }
}
