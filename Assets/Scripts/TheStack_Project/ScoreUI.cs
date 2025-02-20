using TMPro;
using UnityEngine.UI;

namespace TheStack_Project
{
    public class ScoreUI : BaseUI
    {
        private TextMeshProUGUI scoreText;
        private TextMeshProUGUI comboText;
        private TextMeshProUGUI bestScoreText;
        private TextMeshProUGUI bestComboText;

        private Button startButton;
        private Button exitButton;

        protected override UIState GetUIState()
        {
            return UIState.Score;
        }

        public override void Init(UIManager uiManager)
        {
            base.Init(uiManager);

            scoreText = transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
            comboText = transform.Find("ComboText").GetComponent<TextMeshProUGUI>();
            bestScoreText = transform.Find("BestScoreText").GetComponent<TextMeshProUGUI>();
            bestComboText = transform.Find("BestComboText").GetComponent<TextMeshProUGUI>();

            startButton = transform.Find("StartButton").GetComponent<Button>();
            exitButton = transform.Find("ExitButton").GetComponent<Button>();

            startButton.onClick.AddListener(OnClickStartButton);
            exitButton.onClick.AddListener(OnClickExitButton);
        }

        public void SetUI(int score, int combo, int bestScore, int bestCombo)
        {
            scoreText.text = score.ToString();
            comboText.text = combo.ToString();
            bestScoreText.text = bestScore.ToString();
            bestComboText.text = bestCombo.ToString();
        }

        private void OnClickStartButton()
        {
            uiManager.OnClickStart();
        }

        private void OnClickExitButton()
        {
            uiManager.OnClickExit();
        }
    }
}