using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    public int SelectedCharacter { get; private set; }
    [SerializeField] private int GameSceneIndex;
    [SerializeField] GameObject[] characters;
    private int playerIndex = 0;

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
            SceneManager.LoadScene(GameSceneIndex);
            EventManager.InvokeEvent(EventType.StartTrivia);
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
