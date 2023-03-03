using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XInput;
using TMPro;
using UnityEngine.LowLevel;
using Unity.VisualScripting;

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
            int randomInt = Random.Range(0, options.Length);
            options[randomInt].text = QuestionHandler.Instance.GetAnswer();
            foreach (var item in options)
            {
                if (item != options[randomInt])
                {
                    item.text = QuestionHandler.Instance.GetFakeAnswers();
                }
            }
        }
    }

    public GameObject playerMesh;
    public int WhichPlayerType;

    private int playerIndex = 0;

    public Gamepad CurrentGamepad { get; private set; }

    [SerializeField] private TMP_Text[] options;
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject firstButton;
    private PlayerInput playerInput;
    private StateMachine stateMachine;
    private Vector2 movementInput;
    private bool currentTurn;

    private bool button;

    public void SetPlayerIndex(int playerIndex)
    {
        this.playerIndex = playerIndex;
    }

    public void ChoosePlayer(int playerIndex)
    {
        WhichPlayerType = playerIndex;
        EventManager.InvokeEvent(EventType.StartGame);
    }

    private void Awake()
    {
        EventManager.InvokeEvent(EventType.JoinPlayer);
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.StartGame, () => OnStart());
    }

    private void Start()
    {
        playerMesh.transform.position = new Vector3(playerIndex + 50 * 100, transform.position.y, transform.position.z);
    }

    private void OnStart()
    {
        EventSystem.current.SetSelectedGameObject(firstButton);
        playerInput = GetComponent<PlayerInput>();

        var device = playerInput.devices[0];
        if (device.GetType() == typeof(XInputController))
        {
            CurrentGamepad = (XInputController)device;
        }

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
            button = false;
            GameManager.Instance.ChangeTurn();
        }
        //Was for testing purposes
        transform.Translate(moveSpeed * Time.deltaTime * new Vector3(movementInput.x, movementInput.y, 0));
    }

    public void RegainDevice()
    {
        GameManager.Instance.LostDevice = this;
        EventManager.InvokeEvent(EventType.RegainDevice);
    }

    public void DeviceLost()
    {
        GameManager.Instance.LostDevice = this;
        EventManager.InvokeEvent(EventType.DeviceLost);
    }

    public void ChangeTurn(InputAction.CallbackContext context) => button = context.ReadValueAsButton();

    public void OnMove(InputAction.CallbackContext context) => movementInput = context.ReadValue<Vector2>();
}
