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
            playerMesh[WhichPlayerType].SetActive(value);
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

    [SerializeField] private GameObject[] playerMesh;
    [SerializeField] private GameObject firstButton;
    [SerializeField] private GameObject uiObject;
    [SerializeField] private GameObject attackUIObject;
    [SerializeField] private TMP_Text[] options;
    [SerializeField] private TMP_Text[] attackOptions;

    private float scoreGain;

    [SerializeField] private bool currentTurn;
    [SerializeField] private float moveSpeed;

    private bool gameStarted = false;
    private PlayerInput playerInput;

    public void Answer(int answer)
    {
        if (options[answer].text == QuestionHandler.Instance.CurrentQuestion.Answer)
        {
            SkipTurn = false;
            StartAttack();
        }
        else
        {
            SkipTurn = true;
            EventManager.InvokeEvent(EventType.Explanation);
            GameManager.Instance.ChangeScore(0.2f, false);
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

    private bool restart = false;
    private void FixedUpdate()
    {
        if (restart)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
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
        Debug.Log(scoreGain);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButton);
        playerInput = GetComponent<PlayerInput>();

        var device = playerInput.devices[0];
        if (device.GetType() == typeof(XInputControllerWindows))
        {
            CurrentGamepad = (XInputControllerWindows)device;
        }
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    public void Restart(InputAction.CallbackContext context) => restart = context.ReadValueAsButton();
}