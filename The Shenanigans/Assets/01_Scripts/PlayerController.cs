using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private Vector2 movementInput;
    private StateMachine stateMachine;
    [SerializeField] private GameObject uiObject;
    private bool currentTurn;
    public bool CurrentTurn
    {
        get
        {
            return currentTurn;
        }
        set
        {
            uiObject.SetActive(value);
            currentTurn = value;
        }
    }
    private bool button;

    private void Awake()
    {
        EventManager.InvokeEvent(EventType.JoinPlayer);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        SetupStateMachine();
    }

    private void SetupStateMachine()
    {
        IState player1 = new Player1State(this);
        IState player2 = new Player2State(this);
        IState player3 = new Player3State(this);
        IState player4 = new Player4State(this);

        stateMachine = new StateMachine();

        stateMachine.AddTransition(new Transition(null, player1, () => GameManager.Instance.IsTurn == 1));
        stateMachine.AddTransition(new Transition(null, player2, () => GameManager.Instance.IsTurn == 2));
        stateMachine.AddTransition(new Transition(null, player3, () => GameManager.Instance.IsTurn == 3));
        stateMachine.AddTransition(new Transition(null, player4, () => GameManager.Instance.IsTurn == 4));

        stateMachine.SwitchState(player1);
    }

    void Update()
    {
        if (!currentTurn) { return; }
        if (button)
        {
            GameManager.Instance.ChangeTurn();
        }
        transform.Translate(new Vector3(movementInput.x, movementInput.y, 0) * moveSpeed * Time.deltaTime);
    }

    public void ChangeTurn(InputAction.CallbackContext context) => button = context.ReadValueAsButton();

    public void OnMove(InputAction.CallbackContext context) => movementInput = context.ReadValue<Vector2>();
}
