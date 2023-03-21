using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XInput;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Video;
using UnityEngine.UI;

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
            attackUIObject.SetActive(false);
            Invoke(nameof(AnswerHandling), 0.7f);
        }
    }

    [SerializeField] private VideoClip[] attackClips;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage image;

    public Gamepad CurrentGamepad { get; private set; }
    public int WhichPlayerType { get; private set; }
    public bool SkipTurn { get; set; }
    public int Score { get; private set; }

    public Vector3 Position;

    [SerializeField] private Button[] triviaButtons = new Button[3];

    [SerializeField] private GameObject[] playerMesh = new GameObject[3];
    [SerializeField] private Image attackImage;
    [SerializeField] private GameObject firstButton;
    [SerializeField] private GameObject uiObject;
    [SerializeField] private GameObject attackUIObject;
    [SerializeField] private TMP_Text[] options;
    [SerializeField] private TMP_Text[] attackOptions;

    private bool restart;

    private float scoreGain;

    [SerializeField] private bool currentTurn = false;
    [SerializeField] private float moveSpeed;

    private bool gameStarted = false;
    private PlayerInput playerInput;

    public void Answer(int answer)
    {
        Question currentQuestion = QuestionHandler.Instance.CurrentQuestion;
        if (options[answer].text == currentQuestion.Answer)
        {
            SkipTurn = false;
            StartAttack();
        }
        else
        {
            SkipTurn = true;
            QuestionHandler.Instance.WrongAnswer(currentQuestion);
            EventManager.InvokeEvent(EventType.Explanation);
            GameManager.Instance.ChangeScore(0.1f, false);
            GameManager.Instance.ChangeTurn();
        }
        AnswerHandling();
    }

    private void StartAttack()
    {
        QuestionHandler.Instance.QuestionText.gameObject.SetActive(false);
        uiObject.SetActive(false);
        List<string> attacks = new();

        switch (WhichPlayerType)
        {
            case 0:
                {
                    attacks = QuestionHandler.Instance.DevAttacks;
                    break;
                }
            case 1:
                {
                    attacks = QuestionHandler.Instance.ArtistAttacks;
                    break;
                }
            case 2:
                {
                    attacks = QuestionHandler.Instance.DesignAttacks;
                    break;
                }
        }

        for (int i = 0; i < attacks.Count; i++)
        {
            attackOptions[i].text = attacks[i];
        }
        attackUIObject.SetActive(true);
    }

    public void AttackWrapper(int index)
    {
        StartCoroutine(Attack(index));
    }

    public IEnumerator Attack(int index)
    {
        switch (index)
        {
            case 0:
                {
                    GameManager.Instance.ChangeScore(scoreGain, true);
                    break;
                }
            case 1:
                {
                    GameManager.Instance.ChangeScore(scoreGain, true);
                    break;
                }
            case 2:
                {
                    GameManager.Instance.ChangeScore(scoreGain, true);
                    break;
                }
            case 3:
                {
                    GameManager.Instance.ChangeScore(scoreGain, true);
                    break;
                }
        }

        InputSystem.DisableDevice(CurrentGamepad);
        image.enabled = true;
        videoPlayer.clip = attackClips[WhichPlayerType];
        videoPlayer.Play();
        attackUIObject.SetActive(false);
        uiObject.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        QuestionHandler.Instance.QuestionText.gameObject.SetActive(true);
        InputSystem.EnableDevice(CurrentGamepad);
        image.enabled = false;
        uiObject.SetActive(true);
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

    private void FixedUpdate()
    {
        if (restart)
        {
            //EventManager.InvokeEvent(EventType.Restart);
            //Restart();
            playerInput.user.UnpairDevices();
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            Destroy(gameObject);
        }
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
        Debug.Log("TestEnable");
        EventManager.AddListener(EventType.StartTrivia, () => GameStart());
    }

    private void OnDisable()
    {
        Debug.Log("TestDisable");
    }

    private void RestartHandling()
    {
        EventManager.RemoveListener(EventType.StartTrivia, () => GameStart());
        foreach (GameObject objects in playerMesh)
        {
            objects.SetActive(false);
        }

        playerInput.user.UnpairDevices();
        uiObject.SetActive(false);
        attackUIObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    private void GameStart()
    {
        Debug.Log(WhichPlayerType);
        attackImage.color = GameManager.Instance.Colours[WhichPlayerType];

        foreach (Button button in triviaButtons)
        {
            ColorBlock colorBlock = button.colors;
            colorBlock.normalColor = GameManager.Instance.Colours[WhichPlayerType];
            button.colors = colorBlock;
        }

        gameStarted = true;

        playerMesh[WhichPlayerType].transform.position = Position;
        playerMesh[WhichPlayerType].SetActive(true);

        if (!currentTurn) { return; }
        Invoke(nameof(AnswerHandling), 0.7f);
        uiObject.SetActive(true);
        playerMesh[WhichPlayerType].SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    private void Restart()
    {
        Position = Vector3.zero;
        WhichPlayerType = 0;
        currentTurn = false;
        RestartHandling();
        Destroy(this.gameObject);
        Destroy(this);
    }

    private void Start()
    {
        WhichPlayerType = 0;
        scoreGain = (1f / QuestionHandler.Instance.Questions.Count);
        Debug.Log(scoreGain);

        EventManager.InvokeEvent(EventType.JoinPlayer);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButton);
        playerInput = GetComponent<PlayerInput>();

        var device = playerInput.devices[0];
        if (device.GetType() == typeof(XInputControllerWindows))
        {
            CurrentGamepad = (XInputControllerWindows)device;
        }

        if (!currentTurn)
        {
            InputSystem.DisableDevice(CurrentGamepad);
        }
    }

    public void Restart(InputAction.CallbackContext context) => restart = context.ReadValueAsButton();
}