using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Question
{
    public string QuestionText;
    public string Answer;
    public List<string> FakeAnswers;
    public List<string> UsedFakeAnswers;
}

public class QuestionHandler : MonoBehaviour
{
    public static QuestionHandler Instance { get; private set; }

    [SerializeField] private List<Question> questions = new();
    [SerializeField] private List<Question> usedQuestions = new();
    [SerializeField] private TMP_Text questionText;

    private Question currentQuestion;

    public Question GetQuestion()
    {
        if (questions.Count == 0) { return null; }

        currentQuestion = questions[Random.Range(0, questions.Count)];

        usedQuestions.Add(currentQuestion);
        questions.Remove(currentQuestion);
        return currentQuestion;
    }

    public void LaunchQuestion()
    {
        questionText.text = GetQuestion()?.QuestionText;
    }

    public string GetAnswer()
    {
        string answer = currentQuestion.Answer;
        return answer;
    }

    private void Awake()
    {
        Instance = this;
    }

    public string GetFakeAnswers()
    {
        if (currentQuestion.FakeAnswers.Count == 0) { return "nothing"; }
        string current = currentQuestion.FakeAnswers[Random.Range(0, currentQuestion.FakeAnswers.Count)];
        currentQuestion.UsedFakeAnswers.Add(current);
        currentQuestion.FakeAnswers.Remove(current);
        return current;
    }

    private void Start()
    {
        LaunchQuestion();
    }
}
