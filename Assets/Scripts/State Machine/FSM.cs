using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM<T>
{
    public T Owner { get => owner; }
    private T owner;
    

    public State<T> CurrentState { get => currentState; private set { } }
    private State<T> currentState;

    public FSM(T owner) { this.owner = owner; }

    public virtual void OnBegin() { }
    public virtual void OnEnd() { }

    public virtual void OnUpdate() 
    {
        if (currentState == null)
            return;

        currentState.OnUpdate();
    }
    
    public void OnChangeState(State<T> newState) 
    {
        if (currentState != null)
            currentState.OnEnd();
        currentState = newState;
        currentState.OnBegin();
    }
}

public class State<T> 
{
    protected FSM<T> owner;
    public State(FSM<T> owner) { this.owner = owner; }

    public virtual void OnBegin() { }
    public virtual void OnEnd() { }

    public virtual void OnUpdate() { }
}
