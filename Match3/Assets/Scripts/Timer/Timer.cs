using UnityEngine;
using BC_Solution;

public class Timer : MonoBehaviour, IStartable
{

    [SerializeField]
    AbstractClockMono clock;

    private float startTime;

    /// <summary>
    /// Max allowed time on the timer. (In seconds)
    /// </summary>
    public float maxAllowedTime;


    public void IStart()
    {
        startTime = clock.CurrentRenderTime;
    }

    public float GetRemainingTime()
    {
        return Mathf.Max(0, maxAllowedTime - ( clock.CurrentRenderTime - startTime));
    }


}
