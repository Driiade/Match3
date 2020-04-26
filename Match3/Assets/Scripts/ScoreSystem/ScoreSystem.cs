
using System;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    private int score;
    /// <summary>
    /// Score / combo
    /// </summary>
    public Action<int, int> OnAddScore;

    public int GetScore()
    {
        return score;
    }

    public float AddScore(int numberOfDestroyedPieces, int comboCount)
    {
        int s = comboCount*(numberOfDestroyedPieces + ((numberOfDestroyedPieces > 4) ? (numberOfDestroyedPieces - 4) * 2 : 0)); //This score system add a bonus for destroying more than 4 pieces

        this.score += s;
        OnAddScore?.Invoke(score, comboCount);
        return score;
    }

}
