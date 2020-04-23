using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A small FSM to help with behaviour
/// </summary>
/// <typeparam name="T"></typeparam>
public class StatedMono<T> : MonoBehaviour, IEnumStateProvider<T> where T: Enum
{

    public abstract class State
    {
        public T stateType;
        public abstract void OnEnter(StatedMono<T> statedMono);
        public abstract void OnUpdate(StatedMono<T> statedMono);
        public abstract void OnExit(StatedMono<T> statedMono);
    }

    public T CurrentStateType { get => CurrentState.stateType; }

    private State CurrentState { get; set; }

    private Dictionary<T, State> states = new Dictionary<T, State>();

    /// <summary>
    /// Add a state to FSM
    /// </summary>
    /// <param name="stateType"></param>
    /// <param name="state"></param>
    public void Add(T stateType, State state)
    {
        state.stateType = stateType;
        states.Add(stateType, state);
    }


    /// <summary>
    /// Called each time a switch of state occur
    /// </summary>
    public Action<T> OnSwitchState;

    public void SwitchTo(T stateType)
    {
        if (CurrentState != null)
            CurrentState.OnExit(this);

        CurrentState = states[stateType];
        CurrentState.OnEnter(this);

        OnSwitchState?.Invoke(CurrentState.stateType);
    }


    /// <summary>
    /// Unity update call each frame
    /// </summary>
    void Update()
    {
        if (CurrentState != null)
            CurrentState.OnUpdate(this);
    }

}
