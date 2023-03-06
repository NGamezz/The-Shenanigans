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
            uiObject.SetActive(value);
            playerMesh[WhichPlayerType].SetActive(value);
            Invoke(nameof(AnswerHandling), 0.7f);
        }
    }

    public Gamepad CurrentGamepad { get; private set; }
    public int WhichPlayerType { get; private set; }
    public bool SkipTurn { get; set; }
    public int Score { get; private set; }

    [SerializeField] private GameObject[] playerMesh;
    [SerializeField] private GameObject firstButton;
    [SerializeField] private GameObject uiObject;
    [SerializeField] private TMP_Text[] options;
    [SerializeField] private bool currentTurn;
    [SerializeField] private float moveSpeed;
    private float scoreGain;
    private bool gameStarted = false;
    private PlayerInput playerInput;
    public void Answer(int answer)
    {
        if (options[answer].text == QuestionHandler.Instance.CurrentQuestion.Answer)
        {
            SkipTurn = false;
            GameManager.Instance.ChangeScore(scoreGain, true);
        }
        else
        {
            SkipTurn = true;
            GameManager.Instance.ChangeScore(scoreGain, false);
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


    public void ChoosePlayer(int playerIndex)
    {
        WhichPlayerType = playerIndex;
    }

    private void AnswerHandling()
    {
        if (!currentTurn) { return; }
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
        gameStarted = true;
        if (!currentTurn) { return; }
        Invoke(nameof(AnswerHandling), 0.7f);
        uiObject.SetActive(true);
        playerMesh[WhichPlayerType].SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    private void Start()
    {
        scoreGain = (1f / QuestionHandler.Instance.Questions.Count);
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

}
