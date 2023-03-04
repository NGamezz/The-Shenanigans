using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    public int SelectedCharacter { get; private set; }
    [SerializeField] GameObject[] characters;
    private int playerIndex = 0;
    [SerializeField] private int GameSceneIndex;

    public void NextCharacter()
    {
        characters[SelectedCharacter].SetActive(false);
        SelectedCharacter = (SelectedCharacter + 1) % characters.Length;
        characters[SelectedCharacter].SetActive(true);
    }

    public void SelectCharacter()
    {
        GameManager.Instance.Players[playerIndex].ChoosePlayer(SelectedCharacter);
        if (playerIndex == GameManager.Instance.Players.Count - 1)
        {
            EventManager.InvokeEvent(EventType.StartTrivia);
            SceneManager.LoadScene(GameSceneIndex);
            return;
        }
        playerIndex++;
        GameManager.Instance.ChangeTurn();
    }

    public void PreviousCharacter()
    {
        characters[SelectedCharacter].SetActive(false);
        SelectedCharacter--;
        if (SelectedCharacter < 0)
        {
            SelectedCharacter = characters.Length - 1;
        }
        characters[SelectedCharacter].SetActive(true);
    }
}
