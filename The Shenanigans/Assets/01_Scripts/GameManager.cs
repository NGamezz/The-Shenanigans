using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private List<PlayerController> players = new();
    public List<PlayerController> Players { get { return players; } }

    [SerializeField] private List<Gamepad> gamepads = new();

    public static PlayerInputManager PlayerInputManager { get; private set; }

    public PlayerController LostDevice { get; set; }

    private bool started = false;

    public int IsTurn { get; private set; }

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

    public void AddGamePad()
    {
        foreach (Gamepad gamepad in Gamepad.all)
        {
            InputSystem.EnableDevice(gamepad);
        }
    }

    private void Awake()
    {
        Instance = this;
        PlayerInputManager = FindObjectOfType<PlayerInputManager>();
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.JoinPlayer, () => PlayerJoined());
        EventManager.AddListener(EventType.DeviceLost, () => OnDeviceLost());
        EventManager.AddListener(EventType.RegainDevice, () => OnRegainDevice());
        EventManager.AddListener(EventType.StartGame, () => OnStart());
    }

    /// <summary>
    /// Disable gamepad when it's not their turn, gonna have to figure out a function for that later.
    /// </summary>
    public void ChangeTurn()
    {
        players[IsTurn - 1].CurrentTurn = false;
        InputSystem.DisableDevice(players[IsTurn - 1].CurrentGamepad);

        IsTurn++;
        if (IsTurn > players.Count)
        {
            IsTurn = 1;
        }
        QuestionHandler.Instance.LaunchQuestion();

        players[IsTurn - 1].CurrentTurn = true;
        InputSystem.EnableDevice(players[IsTurn - 1].CurrentGamepad);
    }

    private void FixedUpdate()
    {
        if (players.Count == 0) return;
        if (started) { return; }
        EventManager.InvokeEvent(EventType.StartGame);
    }

    private void OnStart()
    {
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
                    player.SetPlayerIndex(IsTurn - 1);
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