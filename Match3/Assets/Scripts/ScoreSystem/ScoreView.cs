using UnityEngine;
using TMPro;
using BC_Solution;

/// <summary>
/// View for the timer
/// </summary>
public class ScoreView : MonoBehaviour, IAwakable, IStartable
{
    [SerializeField]
    TextMeshProUGUI scoreText;

    [SerializeField]
    TextMeshProUGUI comboText;

    [SerializeField]
    float[] comboSizes;

    [SerializeField]
    AbstractClockMono clock;

    private ScoreSystem scoreSystem;
    private float timerCombo;

    public void IAwake()
    {
        scoreText.text = "0";
        comboText.text = "";
    }

    public void IStart()
    {
        scoreSystem = ServiceProvider.GetService<ScoreSystem>();
        scoreSystem.OnAddScore += UpdateView;
        this.enabled = true;
    }

    void OnDestroy()
    {
        if(scoreSystem)
            scoreSystem.OnAddScore -= UpdateView;
    }

    void Update()
    {
        if(timerCombo < clock.CurrentRenderTime)
            comboText.text = "";
    }

    // Update is called once per frame
    void UpdateView(int score, int combo)
    {
        scoreText.text = score.ToString();
        comboText.text = $"X{combo}";

        if (combo >= 8)
            comboText.fontSize = comboSizes[2];
        else if (combo >= 3)
            comboText.fontSize = comboSizes[1];
        else
            comboText.fontSize = comboSizes[0];

        timerCombo = clock.CurrentRenderTime + 2f;
    }
}
