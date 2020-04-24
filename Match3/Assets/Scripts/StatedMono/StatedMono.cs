using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Used with StatedMonoSystem
/// </summary>
public abstract class StatedMono : MonoBehaviour
{
    public abstract void CheckForNextState();
    public abstract void UpdateBehaviour();
}



/// <summary>
/// A small FSM to help with behaviour
/// For this application it run in a frame dependent way. We don't need frame independent algorithm, so keep it like this to save CPU power.
/// </summary>
/// <typeparam name="T"></typeparam>
public class StatedMono<T> : StatedMono, IEnumStateProvider<T> where T: Enum
{

    public abstract class State
    {
        public T stateType;
        public abstract void OnEnter(StatedMono<T> statedMono);
        public abstract void OnUpdate(StatedMono<T> statedMono);
        public abstract void OnExit(StatedMono<T> statedMono);
        public abstract T CheckForNextState(StatedMono<T> statedMono);
    }

    public T CurrentStateType { get => CurrentState != null? CurrentState.stateType : default(T) ; }

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

    private void SwitchTo(T stateType)
    {
        if (CurrentState != null)
            CurrentState.OnExit(this);

        CurrentState = states[stateType];
        CurrentState.OnEnter(this);

        OnSwitchState?.Invoke(CurrentState.stateType);
    }

    public void StartBehaviour(T startState)
    {
        if (CurrentState == null)
            SwitchTo(startState);
        else
            throw new Exception("Trying to start an already running stated behaviour, this is not allowed by design"); //Hard crash the app if you do this
    }


    public override void CheckForNextState()
    {
        if (CurrentState != null)
        {
            T nextState = CurrentState.CheckForNextState(this);
            if (!nextState.Equals(CurrentStateType))
                SwitchTo(nextState);
        }
    }

    /// <summary>
    /// Unity update call each frame
    /// </summary>
    public override  void UpdateBehaviour()
    {
        if (CurrentState != null)
        {
            CurrentState.OnUpdate(this);
        }
    }

}
