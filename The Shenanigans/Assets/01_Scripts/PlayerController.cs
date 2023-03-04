using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XInput;
using TMPro;

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
            if (!gameStarted) { return; }
            playerMesh[WhichPlayerType].SetActive(value);
            Invoke(nameof(AnswerHandling), 0.7f);
        }
    }

    public bool Correct;
    private bool gameStarted = false;
    public int Score { get; private set; }
    [SerializeField] private GameObject[] playerMesh;
    public int WhichPlayerType { get; private set; }
    public Gamepad CurrentGamepad { get; private set; }

    [SerializeField] private TMP_Text[] options;
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject firstButton;
    [SerializeField] private GameObject uiObject;
    private PlayerInput playerInput;
    private bool currentTurn;

    private bool button;

    public void ChoosePlayer(int playerIndex)
    {
        WhichPlayerType = playerIndex;
    }

    private void AnswerHandling()
    {
        int randomInt = Random.Range(0, options.Length);
        options[randomInt].text = QuestionHandler.Instance.GetAnswer();
        foreach (var item in options)
        {
            if (item != options[randomInt])
            {
                item.text = QuestionHandler.Instance.GetFakeAnswers();
            }
        }
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    private void Awake()
    {
        EventManager.InvokeEvent(EventType.JoinPlayer);
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.StartTrivia, () => GameStart());
    }

    private void GameStart()
    {
        Debug.Log("Player Start");
        Invoke(nameof(AnswerHandling), 0.7f);
        gameStarted = true;
        uiObject.SetActive(true);
        playerMesh[WhichPlayerType].SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButton);
        playerInput = GetComponent<PlayerInput>();

        var device = playerInput.devices[0];
        if (device.GetType() == typeof(XInputControllerWindows))
        {
            CurrentGamepad = (XInputControllerWindows)device;
        }
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Answer(int answer)
    {
        if (options[answer].text == QuestionHandler.Instance.CurrentQuestion.Answer)
        {
            Correct = true;
        }
        else
        {
            Correct = false;
        }
        GameManager.Instance.ChangeTurn();
        AnswerHandling();
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
}
