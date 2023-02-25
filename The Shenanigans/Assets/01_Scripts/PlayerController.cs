using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public bool CurrentTurn
    {
        get
        {
            return currentTurn;
        }
        set
        {
            currentTurn = value;
            EventSystem.current.SetSelectedGameObject(firstButton);
        }
    }

    public Gamepad CurrentGamepad { get; private set; }
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject firstButton;
    private PlayerInput playerInput;
    private StateMachine stateMachine;
    private Vector2 movementInput;
    private bool currentTurn;

    private bool button;

    private void Awake()
    {
        EventManager.InvokeEvent(EventType.JoinPlayer);
    }

    private void Start()
    {
        //if (playerInput.devices[0] != null)
        //{
        //    var device = playerInput.devices[0];
        //    if (device.GetType() == typeof(Gamepad))
        //    {
        //        CurrentGamepad = (Gamepad)device;
        //        GameManager.Instance.AddGamePad(CurrentGamepad);
        //    }
        //}

        playerInput = GetComponent<PlayerInput>();
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

    private void Update()
    {
        if (!currentTurn) { return; }
        if (button)
        {
            GameManager.Instance.ChangeTurn();
        }
        transform.Translate(new Vector3(movementInput.x, movementInput.y, 0) * moveSpeed * Time.deltaTime);
    }

    public void RegainDevice()
    {
        GameManager.Instance.LostDevice = this;
        EventManager.InvokeEvent(EventType.RegainDevice);
    }

    public void JoinPlayer(InputAction.CallbackContext context) => GameManager.playerInputManager.JoinPlayerFromActionIfNotAlreadyJoined(context);

    public void DeviceLost()
    {
        GameManager.Instance.LostDevice = this;
        EventManager.InvokeEvent(EventType.DeviceLost);
    }

    public void ChangeTurn(InputAction.CallbackContext context) => button = context.ReadValueAsButton();

    public void OnMove(InputAction.CallbackContext context) => movementInput = context.ReadValue<Vector2>();
}
