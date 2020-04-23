using System;
using UnityEngine;

public class StatedMono<T> : MonoBehaviour, IEnumStateProvider<T> where T: Enum
{
    public T CurrentState { get; private set; }

    /// <summary>
    /// Called each time a switch of state occur
    /// </summary>
    public Action<T> OnSwitchState;

    public void SwitchTo(T state)
    {
        CurrentState = state;
        OnSwitchState?.Invoke(state);
    }

}
