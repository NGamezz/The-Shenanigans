using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XInput;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Xml.Serialization;

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
            if (highLights[WhichPlayerType] == null) { return; }
            highLights[WhichPlayerType].SetActive(value);
            uiObject.SetActive(value);
            attackUIObject.SetActive(false);
            Invoke(nameof(AnswerHandling), 0.6f);
        }
    }

    [SerializeField] private AudioClip[] correctOrWrongAudio;
    [SerializeField] private AudioSource audioSource;

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
    [SerializeField] private GameObject[] highLights = new GameObject[3];
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
            audioSource.clip = correctOrWrongAudio[0];
            audioSource.Play();
            StartAttack();
        }
        else
        {
            audioSource.clip = correctOrWrongAudio[1];
            audioSource.Play();
            SkipTurn = true;
            QuestionHandler.Instance.WrongAnswer(currentQuestion);
            QuestionHandler.Instance.LaunchExplanation(WhichPlayerType);
            GameManager.Instance.ChangeScore(0.1f, false);
            GameManager.Instance.ChangeTurn();
        }
        AnswerHandling();
    }

    private void StartAttack()
    {
        EventManager.InvokeEvent(EventType.Combat);
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

        yield return new WaitForSeconds(2f);

        QuestionHandler.Instance.QuestionText.gameObject.SetActive(true);
        InputSystem.EnableDevice(CurrentGamepad);
        image.enabled = false;
        uiObject.SetActive(true);
        GameManager.Instance.ChangeTurn();
        EventManager.InvokeEvent(EventType.Question);
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
            EventManager.InvokeEvent(EventType.Restart);
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

    private void DisableImage()
    {
        image.enabled = false;
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.Victory, DisableImage);
        EventManager.AddListener(EventType.StartTrivia, GameStart);
        EventManager.AddListener(EventType.Restart, Restart);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.Victory, DisableImage);
        EventManager.RemoveListener(EventType.StartTrivia, GameStart);
        EventManager.RemoveListener(EventType.Restart, Restart);
    }

    private void RestartHandling()
    {
        playerInput.user.UnpairDevices();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        Destroy(gameObject);

        foreach (GameObject objects in playerMesh)
        {
            objects.SetActive(false);
        }

        uiObject.SetActive(false);
        attackUIObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    private void GameStart()
    {
        if (highLights[WhichPlayerType] == null) { return; }
        highLights[WhichPlayerType].SetActive(currentTurn);
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
        Invoke(nameof(AnswerHandling), 0.6f);
        uiObject.SetActive(true);
        playerMesh[WhichPlayerType].SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    private void Restart()
    {
        RestartHandling();
    }

    private void Start()
    {
        WhichPlayerType = 0;
        scoreGain = (1f / QuestionHandler.Instance.Questions.Count);

        EventManager.InvokeEvent(EventType.JoinPlayer);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButton);
        playerInput = GetComponent<PlayerInput>();

        var device = playerInput.devices[0];
        if (device.GetType() == typeof(XInputController))
        {
            CurrentGamepad = (XInputController)device;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }

        if (!currentTurn)
        {
            InputSystem.DisableDevice(CurrentGamepad);
        }
    }

    public void Restart(InputAction.CallbackContext context) => restart = context.ReadValueAsButton();
}