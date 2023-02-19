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
    [SerializeField] private GameObject questionDialog;
    [SerializeField] private GameObject gameEndingDialog;
    private TextMeshProUGUI questionDialogTitle;
    private TextMeshProUGUI questionQuestionText;
    private Button questionYesButton;
    private TextMeshProUGUI questionYesButtonText;
    private Button questionNoButton;
    private TextMeshProUGUI questionNoButtonText;

    private TextMeshProUGUI endingDialogTitle;
    private TextMeshProUGUI endingDialogText;
    private Button endingOkButton;
    private TextMeshProUGUI endingScoreLevel;
    private TextMeshProUGUI endingScoreGold;
    private TextMeshProUGUI endingScoreTime;


    private void Awake() {
        Instance = this;

        questionDialogTitle = menuCanvas.gameObject.transform.Find("QuestionDialogUI/Dialog/DialogTitle/DialogTitleText").GetComponent<TextMeshProUGUI>();
        questionQuestionText = menuCanvas.gameObject.transform.Find("QuestionDialogUI/Dialog/QuestionText").GetComponent<TextMeshProUGUI>();
        questionYesButton = menuCanvas.gameObject.transform.Find("QuestionDialogUI/Dialog/YesButton").GetComponent<Button>();
        questionYesButtonText = menuCanvas.gameObject.transform.Find("QuestionDialogUI/Dialog/YesButton/YesButtonText").GetComponent<TextMeshProUGUI>();
        questionNoButton = menuCanvas.gameObject.transform.Find("QuestionDialogUI/Dialog/NoButton").GetComponent<Button>();
        questionNoButtonText = menuCanvas.gameObject.transform.Find("QuestionDialogUI/Dialog/NoButton/NoButtonText").GetComponent<TextMeshProUGUI>();

        endingDialogTitle = menuCanvas.gameObject.transform.Find("QuestionDialogUI/GameEndingDialog/DialogTitle/DialogTitleText").GetComponent<TextMeshProUGUI>();
        endingDialogText = menuCanvas.gameObject.transform.Find("QuestionDialogUI/GameEndingDialog/EndingText").GetComponent<TextMeshProUGUI>();
        endingOkButton = menuCanvas.gameObject.transform.Find("QuestionDialogUI/GameEndingDialog/YesButton").GetComponent<Button>();
        endingScoreLevel = menuCanvas.gameObject.transform.Find("QuestionDialogUI/GameEndingDialog/ScoreText/ScoreDifficultyLevel").GetComponent<TextMeshProUGUI>();
        endingScoreGold = menuCanvas.gameObject.transform.Find("QuestionDialogUI/GameEndingDialog/ScoreText/ScoreGoldCollected").GetComponent<TextMeshProUGUI>();
        endingScoreTime = menuCanvas.gameObject.transform.Find("QuestionDialogUI/GameEndingDialog/ScoreText/ScoreTimeElapsed").GetComponent<TextMeshProUGUI>();

        Hide();
        transform.SetAsLastSibling();
    }

    private void Hide() {
        questionDialog.SetActive(true);
        gameEndingDialog.SetActive(false);        
        gameObject.SetActive(false);
        // Returns the 'yes' button to its default position
        questionYesButton.transform.localPosition = new Vector3(135, -140, 0);
        questionYesButton.onClick.RemoveAllListeners();
        questionNoButton.onClick.RemoveAllListeners();
        endingOkButton.onClick.RemoveAllListeners();
    }

    public void ShowConfirm(string newDialogTitle, string newQuestionText, string newYesButtonText, string newNoButtonText, Action yesAction, Action noAction) {
        questionDialog.SetActive(true);
        gameEndingDialog.SetActive(false);
        gameObject.SetActive(true);

        questionDialogTitle.text = newDialogTitle;
        questionQuestionText.text = newQuestionText;
        questionYesButtonText.text = newYesButtonText;
        questionNoButtonText.text = newNoButtonText;
        questionYesButton.onClick.AddListener(() => {
            Hide();
            yesAction();
        });
        questionNoButton.onClick.AddListener(() => {
            Hide();
            noAction();
        });
    }

    public void ShowAlert(string newDialogTitle, string newQuestionText, string newOKButtonText, Action yesAction) {
        questionDialog.SetActive(true);
        gameEndingDialog.SetActive(false);
        gameObject.SetActive(true);

        questionDialogTitle.text = newDialogTitle;
        questionQuestionText.text = newQuestionText;
        // If we are showing an 'alert' type of dialog, we need to center the 'yes' button
        // Position is relative to anchorage, values taken from Unity Editor
        questionYesButton.transform.localPosition = new Vector3(0, -140, 0);
        questionYesButtonText.text = newOKButtonText;
        questionYesButton.onClick.AddListener(() => {
            questionNoButton.gameObject.SetActive(true);
            Hide();
            yesAction();
        });
        questionNoButton.gameObject.SetActive(false);
    }
    public void ShowFinishedGameAlert(string newDialogTitle, string newEndingText, Action OkAction) {
        questionDialog.SetActive(false);
        gameEndingDialog.SetActive(true);
        gameObject.SetActive(true);

        endingDialogTitle.text = newDialogTitle;
        endingDialogText.text = newEndingText;
        endingScoreLevel.text = GameProgressManager.GetDifficultyLevel().ToString();
        endingScoreGold.text = GameProgressManager.GetGoldCollected().ToString();
        endingScoreTime.text = GameProgressManager.GetTimeElapsed();
        endingOkButton.onClick.AddListener(() => {
            Hide();
            OkAction();
        });
    }
}
