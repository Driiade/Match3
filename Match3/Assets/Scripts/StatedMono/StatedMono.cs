
using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A small FSM to help with behaviour
/// For this application it run in a frame dependent way. We don't need frame independent algorithm, so keep it like this to save CPU power.
/// </summary>
/// <typeparam name="T"></typeparam>
public class StatedMono<T> : MonoBehaviour, IStated, IEnumStateProvider<T> where T: Enum
{
    public abstract class State
    {
        public T stateType;
        public abstract void OnEnter(StatedMono<T> statedMono);
        public abstract void OnUpdate(StatedMono<T> statedMono);
        public abstract void OnExit(StatedMono<T> statedMono);
        public abstract T CheckForNextState(StatedMono<T> statedMono);
    }


    private bool forceStop = false;

    public bool isRunning => !isPaused && (CurrentState != null || NextState != null);
    public bool isPaused
    {
        get;
        private set;
    }

    public T CurrentStateType { get => CurrentState != null? CurrentState.stateType : default(T) ; }

    protected State LastState { get; private set; }
    protected State CurrentState { get; private set; }
    protected State NextState { get; private set; }

    private Dictionary<T, State> states = new Dictionary<T, State>();

    void OnDestroy()
    {
        if(isRunning)
            StopBehaviour();
    }

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
    public Action<T> OnEnterState;


    public void StartBehaviour(T startState)
    {
        if (!isRunning)
        {
            forceStop = false;
            LastState = null;
            NextState = states[startState];
            IStatedUtils.OnStartBehaviour?.Invoke(this);
        }
        else
            throw new Exception("Trying to start an already running stated behaviour, this is not allowed by design"); //Hard crash the app if you do this
    }


    public void CheckForNextState()
    {
        if (CurrentState != null)
        {
            if (forceStop)
            {
                CurrentState.OnExit(this);
                CurrentState = null;
            }
            else
            {
                NextState = states[CurrentState.CheckForNextState(this)];
                if (NextState != CurrentState)
                {
                    CurrentState.OnExit(this);
                }
            }
        }
    }

    public void CheckForEnteringState()
    {
        if(NextState != CurrentState)
        {
            LastState = CurrentState;

            CurrentState = NextState;
            if (CurrentState != null)
            {
                CurrentState.OnEnter(this);
            }

            OnEnterState?.Invoke(CurrentState.stateType);
            NextState = null;
        }
    }

    /// <summary>
    /// Unity update call each frame
    /// </summary>
    public void UpdateBehaviour()
    {
        if (CurrentState != null)
        {
            CurrentState.OnUpdate(this);
        }
    }

    public void Pause()
    {
        isPaused = true;
        IStatedUtils.OnPauseBehaviour?.Invoke(this);
    }

    public void StopBehaviour()
    {
        forceStop = true;
        NextState = null;
        IStatedUtils.OnStopBehaviour?.Invoke(this);
    }
}
