using UnityEngine;
using UnityEngine.UI;

public class buttonReference : MonoBehaviour
{
    private Button button;
    [SerializeField] private int index = 0;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        CharacterSelection characterSelect = FindObjectOfType<CharacterSelection>();
        button.onClick.AddListener(() =>
        {
            characterSelect.SelectCharacter(index);
        });
    }

}
