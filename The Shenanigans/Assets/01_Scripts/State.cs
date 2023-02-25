using UnityEngine;

public interface IState
{
    public void OnEnter() { }
    public void OnExit() { }
    public void OnUpdate() { }
}

public abstract class State<T> : IState where T : MonoBehaviour
{
    public T Controller { get; private set; }

    public State(T controller)
    {
        Controller = controller;
    }

    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void OnUpdate();
}
