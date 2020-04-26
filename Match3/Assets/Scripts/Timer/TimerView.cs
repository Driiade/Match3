using UnityEngine;
using TMPro;
using System;
using BC_Solution;

/// <summary>
/// View for the timer
/// </summary>
public class TimerView : MonoBehaviour, IStartable
{
    [SerializeField]
    TextMeshProUGUI text;

    private Timer timer;

    public void IStart()
    {
        timer = ServiceProvider.GetService<Timer>();
        this.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        text.text = String.Format("{0:0.00}", timer.GetRemainingTime());
    }
}
