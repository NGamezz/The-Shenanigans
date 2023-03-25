using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static PlayerInputManager PlayerInputManager { get; private set; }
    public List<PlayerController> Players { get { return players; } }
    public static GameManager Instance { get; private set; }
    public PlayerController LostDevice { get; set; }
    public int IsTurn { get; private set; }

    [SerializeField] private List<PlayerController> players = new();

    [SerializeField] private Material spriteRendererMaterial;


    [SerializeField] private List<Gamepad> gamepads = new();

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private GameObject cartridgeObject;

    [SerializeField] private Slider healthSlider;

    public Color[] Colours = new Color[3];

    [SerializeField] private AudioClip battle;
    [SerializeField] private AudioClip trivia;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private GameObject victory;

    [SerializeField] private Vector3 offSet;

    private int height = 0;
    private int width = 0;

    private bool started = false;

    private float score = 0;

    private void Combat()
    {
        if (audioSource == null) { return; }
        audioSource.clip = battle;
        audioSource.Play();
    }

    private void Trivia()
    {
        if (audioSource == null) { return; }
        audioSource.clip = trivia;
        audioSource.Play();
    }

    public void AddGamePad()
    {
        foreach (Gamepad gamepad in Gamepad.all)
        {
            InputSystem.EnableDevice(gamepad);
        }
    }

    public void ChangeTurn()
    {
        QuestionHandler.Instance.LaunchQuestion();
        if (players.Count < 2) { return; }

        players[IsTurn - 1].CurrentTurn = false;
        InputSystem.DisableDevice(players[IsTurn - 1].CurrentGamepad);

        IsTurn++;
        ResetIsTurn();

        if (players[IsTurn - 1].SkipTurn)
        {
            IsTurn++;
            ResetIsTurn();
            CheckSkipTurns();
        }

        players[IsTurn - 1].CurrentTurn = true;
        InputSystem.EnableDevice(players[IsTurn - 1].CurrentGamepad);
    }

    private void RestartHandling()
    {
        score = 0;
        started = false;
        victory.SetActive(false);
        cartridgeObject.SetActive(false);
        IsTurn = 0;
    }

    public void ChangeScore(float change, bool add)
    {
        score = add ? score + change : score - change;
        healthSlider.value = add ? healthSlider.value - change : healthSlider.value + change;

        if (score >= 1)
        {
            score = 1;
            EventManager.InvokeEvent(EventType.Victory);
            Invoke(nameof(RestartTrigger), 2f);
            cartridgeObject.SetActive(false);
            healthSlider.gameObject.SetActive(false);
            victory.SetActive(true);
        }
        if (score <= -0.5f)
        {
            score = -0.5f;
        }
        spriteRendererMaterial.SetFloat("_Dissolve", score);
    }

    private void ResetIsTurn()
    {
        if (IsTurn > players.Count)
        {
            IsTurn = 1;
        }
    }

    private void OnRegainDevice()
    {
        if (LostDevice == null) { return; }
        players.Add(LostDevice);
        UpdateGamePads();
    }

    private void OnDeviceLost()
    {
        if (LostDevice == null) { return; }
        players.Remove(LostDevice);
    }

    private void Start()
    {
        healthSlider.value = healthSlider.maxValue;
        width = Screen.currentResolution.width;
        height = Screen.currentResolution.height;
        IsTurn = 1;
        AddGamePad();
    }

    private void UpdateGamePads()
    {
        foreach (Gamepad gamepad in Gamepad.all)
        {
            if (gamepads.Contains(gamepad)) { break; }
            gamepads.Add(gamepad);
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
            Instance = this;
        }
        if (Instance == null)
        {
            Instance = this;
        }

        PlayerInputManager = FindObjectOfType<PlayerInputManager>();
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.Combat, Combat);
        EventManager.AddListener(EventType.Question, Trivia);
        EventManager.AddListener(EventType.Restart, OnRestart);
        EventManager.AddListener(EventType.JoinPlayer, PlayerJoined);
        EventManager.AddListener(EventType.DeviceLost, OnDeviceLost);
        EventManager.AddListener(EventType.RegainDevice, OnRegainDevice);
        EventManager.AddListener(EventType.StartGame, OnStart);
        EventManager.AddListener(EventType.StartTrivia, StartTrivia);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.Combat, Combat);
        EventManager.RemoveListener(EventType.Question, Trivia);
        EventManager.RemoveListener(EventType.Restart, OnRestart);
        EventManager.RemoveListener(EventType.JoinPlayer, PlayerJoined);
        EventManager.RemoveListener(EventType.DeviceLost, OnDeviceLost);
        EventManager.RemoveListener(EventType.RegainDevice, OnRegainDevice);
        EventManager.RemoveListener(EventType.StartGame, OnStart);
        EventManager.RemoveListener(EventType.StartTrivia, StartTrivia);
    }

    private void StartTrivia()
    {
        if (this == null) { return; }
        EventManager.InvokeEvent(EventType.Question);
        PlayerInputManager.DisableJoining();

        int playerIndex = 0;
        foreach (PlayerController player in players)
        {
            player.Position.x = (-width / 2 / 100) + (playerIndex + (width / 12 / 100)) * offSet.x;
            player.Position.y = (-height / 5 / 100);
            playerIndex++;
        }

        if (cartridgeObject == null)
        {
            Debug.Log(this);
            cartridgeObject = GetComponentInChildren<Boss>().gameObject;
            Debug.Log(cartridgeObject);
        }

        cartridgeObject.SetActive(true);
        IsTurn = 1;
        foreach (PlayerController player in players)
        {
            if (player == players[IsTurn - 1])
            {
                InputSystem.EnableDevice(player.CurrentGamepad);
                player.CurrentTurn = true;
            }
            else
            {
                InputSystem.DisableDevice(player.CurrentGamepad);
                player.CurrentTurn = false;
            }
        }
        Invoke(nameof(GetSpriteRenderer), 0.4f);
    }

    private void GetSpriteRenderer()
    {
        spriteRenderer = FindObjectOfType<Boss>().GetComponent<SpriteRenderer>();
        spriteRendererMaterial = spriteRenderer.material;
    }

    private void CheckSkipTurns()
    {
        foreach (PlayerController player in players)
        {
            player.SkipTurn = false;
        }
    }

    private void FixedUpdate()
    {
        if (players.Count == 0) return;
        if (started) { return; }
        EventManager.InvokeEvent(EventType.StartGame);
    }

    private void RestartTrigger()
    {
        EventManager.InvokeEvent(EventType.Restart);
    }

    private void OnRestart()
    {
        if (this == null) { return; }
        PlayerInputManager inputManager = FindObjectOfType<PlayerInputManager>();
        inputManager.EnableJoining();
        healthSlider.value = healthSlider.maxValue;
        score = 0;
        gamepads.Clear();
        cartridgeObject.SetActive(false);
        players.Clear();
        spriteRendererMaterial.SetFloat("_Dissolve", 0f);
    }

    private void OnStart()
    {
        foreach (PlayerController player in players)
        {
            if (player == players[0])
            {
                player.CurrentTurn = true;
            }
            else
            {
                player.CurrentTurn &= false;
            }
        }

        foreach (Gamepad gamepad in gamepads)
        {
            InputSystem.EnableDevice(gamepad);
        }
        players[IsTurn - 1].CurrentTurn = true;
        started = true;
    }

    private void PlayerJoined()
    {
        PlayerController[] _players = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in _players)
        {
            if (players.Contains(player))
            {
                if (player == players[IsTurn - 1])
                {
                    player.CurrentTurn = true;
                }
                else
                {
                    player.CurrentTurn = false;
                }
                return;
            }
            players.Add(player);
        }
    }
}