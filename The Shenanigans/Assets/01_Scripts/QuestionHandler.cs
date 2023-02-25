using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Question
{
    public string QuestionText;
    public string Answer;
    public bool Used = false;
}


public class QuestionHandler : MonoBehaviour
{
    public static QuestionHandler Instance { get; private set; }

    [SerializeField] private List<Question> questions = new();

    [SerializeField] private TMP_Text questionText;

    public Question GetQuestion(int index, bool random)
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
    public void LaunchQuestion(int index, bool random)
    {
        questionText.text = GetQuestion(index, random).QuestionText;
    }

    public string GetAnswer(int index)
    {
        string answer = questions[index].Answer;
        return answer;
    }

    private void Start()
    {
        LaunchQuestion(0, true);
    }
}
