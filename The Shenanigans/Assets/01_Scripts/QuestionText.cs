using UnityEngine;
using TMPro;

public class QuestionText : MonoBehaviour
{
    private TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        QuestionHandler.Instance.SetQuestionObject(text);
    }
}
