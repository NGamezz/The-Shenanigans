
using System;

public class Transition
{
    public IState previousState;
    public IState nextState;

    public Func<bool> condition;

    public Transition(IState previousState, IState nextState, Func<bool> condition)
    {
        this.previousState = previousState;
        this.nextState = nextState;
        this.condition = condition;
    }

    public bool ReturnJudgement()
    {
        return condition();
    }
}
