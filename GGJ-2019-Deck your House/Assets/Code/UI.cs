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

    [SerializeField]
    private TextMeshProUGUI instructions;
    [SerializeField]
    private TextMeshProUGUI guide;
    [SerializeField]
    private TextMeshProUGUI buttonText;

    [SerializeField]
    private GameObject[] children;

    public void UpdateText(string instructions, string buttonText)
    {
        this.instructions.text = instructions;
        this.buttonText.text = buttonText;
    }

    public void UpdateGuide(string guideText)
    {
        guide.gameObject.SetActive(true);
        guide.text = guideText;
    }

    public void ShowUI(bool value)
    {
        foreach (GameObject go in children)
        {
            go.SetActive(value);
        }
    }
}
