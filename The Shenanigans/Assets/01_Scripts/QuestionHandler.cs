using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Question
{
    public string QuestionText;
    public string Answer;
}

public class QuestionHandler : MonoBehaviour
{
    public static QuestionHandler Instance { get; private set; }

    [SerializeField] private List<Question> questions = new();
    [SerializeField] private List<Question> usedQuestions = new();
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private List<string> fakeAnswers = new();
    [SerializeField] private List<string> usedFakeAnswers = new();

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
        if (fakeAnswers.Count == 0) { return "nothing"; }
        string current = fakeAnswers[Random.Range(0, fakeAnswers.Count)];
        usedFakeAnswers.Add(current);
        fakeAnswers.Remove(current);
        return current;
    }

    private void Start()
    {
        LaunchQuestion();
    }
}
