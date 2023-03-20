using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem.XInput;

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

    [SerializeField] private TMP_Text scoreText;

    [SerializeField] private GameObject victory;

    private bool started = false;

    private float score = 0;

    public void AddGamePad()
    {
        foreach (Gamepad gamepad in Gamepad.all)
        {
            InputSystem.EnableDevice(gamepad);
        }
    }

    /// <summary>
    /// Disable gamepad when it's not their turn, gonna have to figure out a function for that later.
    /// </summary>
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
        scoreText.gameObject.SetActive(false);
        IsTurn = 0;
    }

    public void ChangeScore(float change, bool add)
    {
        if (add)
        {
            score += change;
        }
        else
        {
            score -= change;
        }
        if (score >= 1)
        {
            score = 1;
            victory.SetActive(true);
        }
        if (score <= -0.5f)
        {
            score = -0.5f;
        }
        scoreText.text = score.ToString();
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
        scoreText.text = 0.ToString();
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
            Destroy(this);
        }
        if (Instance == null)
        {
            Instance = this;
        }

        PlayerInputManager = FindObjectOfType<PlayerInputManager>();
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.JoinPlayer, PlayerJoined);
        EventManager.AddListener(EventType.DeviceLost, () => OnDeviceLost());
        EventManager.AddListener(EventType.RegainDevice, () => OnRegainDevice());
        EventManager.AddListener(EventType.StartGame, () => OnStart());
        EventManager.AddListener(EventType.StartTrivia, () => StartTrivia());
    }

    private void StartTrivia()
    {
        cartridgeObject.SetActive(true);
        scoreText.gameObject.SetActive(true);
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