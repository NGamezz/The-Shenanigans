using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;
using Unity.VisualScripting;
using UnityEngine.UI;

[System.Serializable]
public class Question
{
    public string QuestionText;
    public string Answer;
    public string Explanation;

    public void Reset()
    {
        FakeAnswers.Clear();
        FakeAnswers.AddRange(UsedFakeAnswers);
        UsedFakeAnswers.Clear();
    }

    public List<string> FakeAnswers = new();
    public List<string> UsedFakeAnswers = new();
}

public class QuestionHandler : MonoBehaviour
{
    public static QuestionHandler Instance { get; private set; }
    public Question CurrentQuestion { get; private set; }

    [SerializeField] private List<Question> usedQuestions = new();

    [SerializeField] private List<Question> wrongQuestions = new();

    [SerializeField] private List<Question> questions = new();
    public List<Question> Questions { get { return questions; } }

    [SerializeField] private TMP_Text explanationText;

    [SerializeField] private int explanationAmount = 5;
    [SerializeField] private TMP_Text questionText;
    public TMP_Text QuestionText { get { return questionText; } }

    [SerializeField] private List<string> devAttacks = new();
    public List<string> DevAttacks { get { return devAttacks; } }
    [SerializeField] private List<string> artistAttacks = new();
    public List<string> ArtistAttacks { get { return artistAttacks; } }
    [SerializeField] private List<string> designAttacks = new();
    public List<string> DesignAttacks { get { return designAttacks; } }

    private readonly Question nullQuestion = new();

    [SerializeField] private GameObject explanationObject;

    private Image explanationBackground;

    private bool victory = false;

    public void LaunchExplanation(int index)
    {
        StartCoroutine(Explanation(explanationAmount, index));
    }

    private IEnumerator Explanation(int delay, int index)
    {
        explanationBackground.color = GameManager.Instance.Colours[index];
        explanationObject.SetActive(true);
        explanationText.text = CurrentQuestion.Explanation;
        yield return new WaitForSeconds(delay);
        explanationObject.SetActive(false);
    }

    public void WrongAnswer(Question question)
    {
        if (!wrongQuestions.Contains(question))
        {
            wrongQuestions.Add(question);
        }
    }

    public Question GetQuestion()
    {
        if (questions.Count == 0)
        {
            if (wrongQuestions.Count == 0)
            {
                HashSet<Question> noDupeQuestions = new();
                noDupeQuestions.AddRange(usedQuestions);
                questions.AddRange(noDupeQuestions);
                usedQuestions.Clear();
            }
            else
            {
                questions.AddRange(wrongQuestions);
                CurrentQuestion = questions[Random.Range(0, questions.Count - 1)];
                CurrentQuestion.Reset();
                wrongQuestions.Clear();
            }
        }
        else
        {
            CurrentQuestion = questions[Random.Range(0, questions.Count - 1)];
        }

        usedQuestions.Add(CurrentQuestion);
        questions.Remove(CurrentQuestion);

        if (CurrentQuestion.FakeAnswers.Count == 0)
        {
            CurrentQuestion.Reset();
        }

        return CurrentQuestion;
    }

    public void SetQuestionObject(TMP_Text textObject)
    {
        questionText = textObject;
    }

    public void LaunchQuestion()
    {
        if (victory)
        {
            questionText.text = "";
        }
        if (questionText == null || victory) { return; }
        questionText.text = GetQuestion().QuestionText;
    }

    public string GetAnswer()
    {
        string answer = CurrentQuestion.Answer;
        return answer;
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.Victory, () => victory = true);
        EventManager.AddListener(EventType.StartTrivia, () => Invoke(nameof(LaunchQuestion), 0.6f));
        EventManager.AddListener(EventType.Restart, Restart);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.Victory, () => victory = true);
        EventManager.RemoveListener(EventType.StartTrivia, () => Invoke(nameof(LaunchQuestion), 0.6f));
        EventManager.RemoveListener(EventType.Restart, Restart);
    }

    private void Restart()
    {
        HashSet<Question> noDupeQuestions = new HashSet<Question>();
        noDupeQuestions.AddRange(usedQuestions);
        questions.Clear();
        usedQuestions.Clear();
        wrongQuestions.Clear();
        questions.AddRange(noDupeQuestions);
        foreach (Question question in questions)
        {
            question.Reset();
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
            Instance = this;
        }
        if (Instance == null)
        {
            Instance = this;
        }
        explanationBackground = explanationObject.GetComponentInChildren<Image>();
    }

    private void Start()
    {
        nullQuestion.QuestionText = "Null";
        nullQuestion.Answer = "Null";
    }

    public string GetFakeAnswers()
    {
        if (CurrentQuestion.FakeAnswers.Count == 0)
        {
            CurrentQuestion.Reset();
        }
        string current = CurrentQuestion.FakeAnswers[Random.Range(0, CurrentQuestion.FakeAnswers.Count)];
        CurrentQuestion.UsedFakeAnswers.Add(current);
        CurrentQuestion.FakeAnswers.Remove(current);
        return current;
    }
}
