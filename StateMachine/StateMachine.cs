using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public List<State> states = new List<State>();
    public State CurrentState = null;
    public Action<State> OnStateChange;

    public void SwitchState<aState>()
    {
        foreach (State s in states)
        {
            if (s.GetType() == typeof(aState))
            {
                CurrentState?.ExitState();
                CurrentState = s;
                CurrentState.EnterState();
                OnStateChange?.Invoke(CurrentState);
                return;
            }
        }

    }

    public virtual void UpdateStateMachine()
    {
        CurrentState?.UpdateState();
    }

    public bool IsState<aState>()
    {
        if (!CurrentState) return false;
        return CurrentState.GetType() == typeof(aState);
    }
}
