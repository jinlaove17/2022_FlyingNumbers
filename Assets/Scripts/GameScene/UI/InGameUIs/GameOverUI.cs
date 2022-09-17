using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : UI
{
    [SerializeField]
    private GameObject ui;

    [SerializeField]
    private GameObject exclamationMark;

    [SerializeField]
    private Text scoreText;

    [SerializeField]
    private Text bestText;

    public override void Init()
    {
        scoreText.gameObject.SetActive(false);
        scoreText.text = "0";
        bestText.gameObject.SetActive(true);
    }

    public void ShowExclamationMark(Vector2 worldPosition)
    {
        exclamationMark.transform.position = Camera.main.WorldToScreenPoint(worldPosition);
        animator.Play("ShowExclamationMark", -1, 0.0f);

        SoundManager.Instance.PlaySFX("ExclamationMark");
    }

    public void ShowGameResult()
    {
        // 다른 UI를 숨긴다.
        switch (UIManager.Instance.PopUpUI)
        {
            case POPUP_UI.GAMEOVER:
                return;
            case POPUP_UI.LEVEL_SELECTION:
                UIManager.Instance.LobbyUIs.LevelSelectionUI.Hide();
                break;
            case POPUP_UI.OPTION:
                UIManager.Instance.LobbyUIs.OptionUI.Hide();
                break;
        }

        UIManager.Instance.PopUpUI = POPUP_UI.GAMEOVER;

        bestText.gameObject.SetActive(false);
        animator.Play("ShowGameResult", -1, 0.0f);
    }

    public void HideGameResult()
    {
        UIManager.Instance.PopUpUI = POPUP_UI.NONE;

        // 애니메이션의 상태를 초기화 해주어야, activeSelf가 애니메이션에 의해 변경되지 않는다.
        animator.Rebind();

        // 다른 UI가 오픈되기 때문에 바로 SetActive로 닫음
        ui.SetActive(false);
    }

    private IEnumerator IncreaseScore(float duration)
    {
        float currentScore = 0.0f;
        float targetScore = UIManager.Instance.InGameUIs.ScoreUI.TargetScore;
        float offset = targetScore / duration;
        Color color = new Color(bestText.color.r, bestText.color.g, bestText.color.b, 0.0f);

        switch (GameManager.Instance.ControlLevel)
        {
            case CONTROL_LEVEL.EASY:
                if (targetScore > DataManager.Instance.UserData.easyHighScore)
                {
                    DataManager.Instance.UserData.easyHighScore = (int)targetScore;
                    UIManager.Instance.LobbyUIs.HighScoreTexts[0].text = targetScore.ToString();

                    bestText.color = color;
                    bestText.gameObject.SetActive(true);
                }
                break;
            case CONTROL_LEVEL.NORMAL:
                if (targetScore > DataManager.Instance.UserData.normalHighScore)
                {
                    DataManager.Instance.UserData.normalHighScore = (int)targetScore;
                    UIManager.Instance.LobbyUIs.HighScoreTexts[1].text = targetScore.ToString();

                    bestText.color = color;
                    bestText.gameObject.SetActive(true);
                }
                break;
            case CONTROL_LEVEL.HARD:
                if (targetScore > DataManager.Instance.UserData.hardHighScore)
                {
                    DataManager.Instance.UserData.hardHighScore = (int)targetScore;
                    UIManager.Instance.LobbyUIs.HighScoreTexts[2].text = targetScore.ToString();

                    bestText.color = color;
                    bestText.gameObject.SetActive(true);
                }
                break;
        }

        while (currentScore < targetScore)
        {
            currentScore += offset * Time.deltaTime;
            color.a += offset * Time.deltaTime;

            scoreText.text = ((int)currentScore).ToString();
            bestText.color = color;

            yield return null;
        }

        currentScore = targetScore;
        scoreText.text = ((int)currentScore).ToString();
        bestText.color = new Color(bestText.color.r, bestText.color.g, bestText.color.b, 1.0f);

        DataManager.Instance.SaveUserData();
    }

    public void OnClickExitButton()
    {
        UIManager.Instance.PopUpUI = POPUP_UI.NONE;

        // 애니메이션에 의해 UI를 닫음.
        animator.Play("HideGameResult", -1, 0.0f);

        SoundManager.Instance.PlaySFX("Panel");
    }
}
