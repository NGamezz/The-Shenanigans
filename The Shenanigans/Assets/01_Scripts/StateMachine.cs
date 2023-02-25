using System.Collections.Generic;
using System;

public class StateMachine
{
    public IState currentState;
    public List<Transition> allTransitions = new List<Transition>();
    public List<Transition> allActiveTransitions = new List<Transition>();

    public void OnFixedUpdate()
    {
        foreach (Transition transition in allActiveTransitions)
        {
            if (transition.ReturnJudgement())
            {
                SwitchState(transition.nextState);
                return;
            }
        }
        currentState?.OnUpdate();
    }
    public void SwitchState(IState state)
    {
        currentState?.OnExit();
        currentState = state;
        allActiveTransitions = allTransitions.FindAll(x => x.previousState == currentState || x.previousState == null);
        currentState?.OnEnter();
    }
    public void OnUpdate()
    {
        currentState?.OnUpdate();
    }
    public void AddTransition(Transition transition)
    {
        allTransitions.Add(transition);
    }
}
