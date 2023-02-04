using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Presents a generic "Confirm" style dialog
// Functions can be passed to excecute actions after a button is pressed
public class QuestionDialogUI : MonoBehaviour
{
    public static QuestionDialogUI Instance { get; private set; }
    [SerializeField] private GameObject menuCanvas;
    private TextMeshProUGUI dialogTitle;
    private TextMeshProUGUI questionText;
    private Button yesButton;
    private TextMeshProUGUI yesButtonText;
    private Button noButton;
    private TextMeshProUGUI noButtonText;

    private void Awake() {
        Instance = this;

        dialogTitle = menuCanvas.gameObject.transform.Find("QuestionDialogUI/Dialog/DialogTitle/DialogTitleText").GetComponent<TextMeshProUGUI>();
        questionText = menuCanvas.gameObject.transform.Find("QuestionDialogUI/Dialog/QuestionText").GetComponent<TextMeshProUGUI>();
        yesButton = menuCanvas.gameObject.transform.Find("QuestionDialogUI/Dialog/YesButton").GetComponent<Button>();
        yesButtonText = menuCanvas.gameObject.transform.Find("QuestionDialogUI/Dialog/YesButton/YesButtonText").GetComponent<TextMeshProUGUI>();
        noButton = menuCanvas.gameObject.transform.Find("QuestionDialogUI/Dialog/NoButton").GetComponent<Button>();
        noButtonText = menuCanvas.gameObject.transform.Find("QuestionDialogUI/Dialog/NoButton/NoButtonText").GetComponent<TextMeshProUGUI>();

        Hide();
        transform.SetAsLastSibling();
    }

    private void Hide() {
        gameObject.SetActive(false);
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
    }

    public void ShowQuestion(string newDialogTitle, string newQuestionText, string newYesButtonText, string newNoButtonText, Action yesAction, Action noAction) {
        gameObject.SetActive(true);

        dialogTitle.text = newDialogTitle;
        questionText.text = newQuestionText;
        yesButtonText.text = newYesButtonText;
        noButtonText.text = newNoButtonText;
        yesButton.onClick.AddListener(() => {
            Hide();
            yesAction();
        });
        noButton.onClick.AddListener(() => {
            Hide();
            noAction();
        });
    }
}
