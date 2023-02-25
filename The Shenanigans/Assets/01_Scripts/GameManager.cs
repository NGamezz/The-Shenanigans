using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private List<PlayerController> players = new();

    [SerializeField] private List<Gamepad> gamepads = new();

    public static PlayerInputManager playerInputManager { get; private set; }

    public PlayerController LostDevice { get; set; }

    private bool started = false;

    public int IsTurn { get; set; }

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
        foreach (Gamepad gamepad in Gamepad.all)
        {
            gamepads.Add(gamepad);
        }
    }

    private void UpdateGamePads()
    {
        foreach (Gamepad gamepad in Gamepad.all)
        {
            bool InList = false;
            for (int i = 0; i < gamepads.Count; i++)
            {
                if (gamepads[i] == gamepad)
                {
                    InList = true;
                }
                if (InList == false)
                {
                    gamepads.Add(gamepad);
                }
            }
        }
    }

    public void AddGamePad(Gamepad gamepad)
    {
        if (gamepads.Contains(gamepad)) { return; }
        gamepads.Add(gamepad);
    }

    private void Awake()
    {
        Instance = this;
        playerInputManager = FindObjectOfType<PlayerInputManager>();
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.JoinPlayer, () => PlayerJoined());
        EventManager.AddListener(EventType.DeviceLost, () => OnDeviceLost());
        EventManager.AddListener(EventType.RegainDevice, () => OnRegainDevice());
    }


    /// <summary>
    /// Disable gamepad when it's not their turn, gonna have to figure out a function for that later.
    /// </summary>
    public void ChangeTurn()
    {
        players[IsTurn - 1].CurrentTurn = false;
        //InputSystem.DisableDevice(device: players[IsTurn - 1].CurrentGamepad);

        IsTurn++;
        if (IsTurn > players.Count)
        {
            IsTurn = 1;
        }

        //InputSystem.EnableDevice(device: players[IsTurn - 1].CurrentGamepad);
        players[IsTurn - 1].CurrentTurn = true;
    }

    private void FixedUpdate()
    {
        //if (playerInputManager.playerCount >= 4)
        //{
        //    playerInputManager.DisableJoining();
        //}

        if (players.Count == 0) return;
        if (started) { return; }
        OnStart();
    }

    private void OnStart()
    {
        foreach (Gamepad gamepad in gamepads)
        {
            InputSystem.EnableDevice(gamepad);
        }

        started = true;
        IsTurn = 1;
        players[IsTurn - 1].CurrentTurn = true;
    }

    private void PlayerJoined()
    {
        PlayerController[] _players = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in _players)
        {
            if (players.Contains(player)) { return; }
            players.Add(player);
        }
    }
}
