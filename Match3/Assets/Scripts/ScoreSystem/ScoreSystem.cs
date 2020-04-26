
using System;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    private int score;
    public Action<int> OnAddScore;

    public int GetScore()
    {
        return score;
    }

    public float AddScore(int numberOfDestroyedPieces)
    {
        int s = numberOfDestroyedPieces + ((numberOfDestroyedPieces > 4) ? (numberOfDestroyedPieces - 4) * 2 : 0); //This score system add a bonus for destroying more than 4 pieces

        this.score += s;
        OnAddScore?.Invoke(score);
        return score;
    }

}
