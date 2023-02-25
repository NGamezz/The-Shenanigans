using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestionHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text questionText;

    public static QuestionHandler Instance { get; private set; }

    [SerializeField] private List<string> questions = new();

    public string GetQuestion(int index, bool random)
    {
        if (random)
        {
            return questions[Random.Range(0, questions.Count)];
        }
        else
        {
            return questions[index];
        }
    }

    private void Start()
    {
        LaunchQuestion();
    }

    public void LaunchQuestion()
    {
        questionText.text = GetQuestion(0, true);
    }
}
