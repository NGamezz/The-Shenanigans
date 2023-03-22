using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] private int GameSceneIndex;
    private int playerIndex = 0;
    [SerializeField] private Button[] buttons = new Button[3];
    private Color[] colours => GameManager.Instance.Colours;

    private int playerType = 0;

    private void Start()
    {
        foreach (Button button in buttons)
        {
            ColorBlock colorBlock = button.colors;
            colorBlock.selectedColor = colours[playerIndex];
            button.colors = colorBlock;
        }
    }

    public void SelectCharacter(int index)
    {
        playerType = index;

        GameManager.Instance.Players[playerIndex].ChoosePlayer(playerType);

        if (playerIndex == GameManager.Instance.Players.Count - 1)
        {
            SceneManager.LoadScene(GameSceneIndex);
            EventManager.InvokeEvent(EventType.StartTrivia);
            return;
        }

        playerIndex++;

        foreach (Button button in buttons)
        {
            ColorBlock colorBlock = button.colors;
            colorBlock.selectedColor = colours[playerIndex];
            button.colors = colorBlock;
        }
        GameManager.Instance.ChangeTurn();
    }
}
