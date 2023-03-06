using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;

[System.Serializable]
public class Question
{
    public string QuestionText;
    public string Answer;

    public List<string> FakeAnswers = new();
    public List<string> UsedFakeAnswers = new();
}

public class QuestionHandler : MonoBehaviour
{
    public static QuestionHandler Instance { get; private set; }
    public Question CurrentQuestion { get; private set; }

    [SerializeField] private List<Question> usedQuestions = new();
    [SerializeField] private List<Question> questions = new();
    public List<Question> Questions { get { return questions; } }
    [SerializeField] private TMP_Text questionText;

    private readonly Question nullQuestion = new();

    public Question GetQuestion()
    {
        if (questions.Count == 0) { return nullQuestion; }

        CurrentQuestion = questions[Random.Range(0, questions.Count)];

        usedQuestions.Add(CurrentQuestion);
        questions.Remove(CurrentQuestion);
        return CurrentQuestion;
    }

    public void SetQuestionObject(TMP_Text textObject)
    {
        questionText = textObject;
    }

    public void LaunchQuestion()
    {
        if (questionText == null) { return; }
        questionText.text = GetQuestion().QuestionText;
    }

    public string GetAnswer()
    {
        string answer = CurrentQuestion.Answer;
        return answer;
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.StartTrivia, () => Starting());
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        if (Instance != this)
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        nullQuestion.QuestionText = "Null";
        nullQuestion.Answer = "Null";
    }

    public string GetFakeAnswers()
    {
        if (CurrentQuestion.FakeAnswers.Count == 0) { return "Null"; }
        string current = CurrentQuestion.FakeAnswers[Random.Range(0, CurrentQuestion.FakeAnswers.Count)];
        CurrentQuestion.UsedFakeAnswers.Add(current);
        CurrentQuestion.FakeAnswers.Remove(current);
        return current;
    }

    private void Starting()
    {
        Invoke(nameof(LaunchQuestion), 0.6f);
    }
}
