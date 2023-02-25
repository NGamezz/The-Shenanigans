using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] public static GameManager Instance { get; private set; }
    [SerializeField] public PlayerController CurrentPlayer { get; set; }

    [SerializeField] private List<PlayerController> players = new();

    private bool started = false;

    public int IsTurn { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.JoinPlayer, () => PlayerJoined());
    }

    public void ChangeTurn()
    {
        players[IsTurn - 1].CurrentTurn = false;
        IsTurn++;
        if (IsTurn > players.Count)
        {
            IsTurn = 1;
        }
        Debug.Log(IsTurn);
        players[IsTurn - 1].CurrentTurn = true;
    }

    private void FixedUpdate()
    {
        if (players.Count == 0) return;
        if (started) { return; }
        Debug.Log("Test");
        OnStart();
    }

    private void OnStart()
    {
        started = true;
        IsTurn = 1;
        players[0].CurrentTurn = true;
    }

    private void PlayerJoined()
    {
        bool isInList = false;
        PlayerController[] _players = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in _players)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] == player)
                {
                    isInList = true;
                }
            }
            if (!isInList)
            {
                players.Add(player);
            }
        }
        Debug.Log(players.Count);
    }
}
