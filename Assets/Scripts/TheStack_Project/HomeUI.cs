using UnityEngine.UI;

namespace TheStack_Project
{
    public class HomeUI : BaseUI
    {
        private Button startButton;
        private Button exitButton;

        protected override UIState GetUIState()
        {
            return UIState.Home;
        }

        public override void Init(UIManager uiManager)
        {
            base.Init(uiManager);

            startButton = transform.Find("StartButton").GetComponent<Button>();
            exitButton = transform.Find("ExitButton").GetComponent<Button>();

            startButton.onClick.AddListener(OnClickStartButton);
            exitButton.onClick.AddListener(OnClickExitButton);
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