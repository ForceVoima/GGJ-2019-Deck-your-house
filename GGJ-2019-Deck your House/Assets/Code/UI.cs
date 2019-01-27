using UnityEngine;
using System.Collections;
using TMPro;

public class UI : MonoBehaviour
{
#region Statics
    private static UI _instance;

    public static UI Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Instantiate(Resources.Load<UI>("UI"));
            }

            return _instance;
        }
    }

    public void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);
    }
    #endregion Statics
    public GameObject instructionsGO;

    [SerializeField]
    private TextMeshProUGUI instructionsText;

    public GameObject buttonGO;

    [SerializeField]
    private TextMeshProUGUI buttonText;

    [SerializeField]
    private GameObject[] children;

    public GameObject headerGO;
    public UnityEngine.UI.Image headerImage;
    public TextMeshProUGUI headerText;

    public Color player1Color;
    public Color player2Color;
    public Color neutralColor;

    public GameObject tutorialGO;

    public GameObject resultsGO;
    public TextMeshProUGUI scoreText;

    public void UpdateHeader(GameManager.TurnPhase turnPhase, string text)
    {
        headerGO.SetActive(true);
        headerText.text = text;

        switch (turnPhase)
        {
            case GameManager.TurnPhase.Wait1:
                headerImage.color = neutralColor;
                break;
            case GameManager.TurnPhase.Player1:
                headerImage.color = player1Color;
                break;
            case GameManager.TurnPhase.Wait2:
                headerImage.color = neutralColor;
                break;
            case GameManager.TurnPhase.Player2:
                headerImage.color = player2Color;
                break;
        }
    }

    public void ShowTutorial()
    {
        tutorialGO.SetActive(true);
    }

    public void ShowUI()
    {
        ShowUI(true);
    }

    public void ClearUI()
    {
        ShowUI(false);
    }

    public void ShowScore(int totalScore)
    {
        resultsGO.SetActive(true);
        scoreText.text = totalScore.ToString();
    }

    public void UpdateInstructions(string instructions, string buttonText)
    {
        instructionsGO.SetActive(true);
        buttonGO.SetActive(true);

        instructionsText.text = instructions;
        this.buttonText.text = buttonText;
    }

    public void UpdateInstructions(string instructions)
    {
        instructionsGO.SetActive(true);
        buttonGO.SetActive(false);

        instructionsText.text = instructions;
    }

    public void ShowUI(bool value)
    {
        foreach (GameObject go in children)
        {
            go.SetActive(value);
        }
    }
}
