using UnityEngine;

public class LevelSelectionUI : UI
{
    [SerializeField]
    private GameObject ui;

    private void Show()
    {
        // 다른 UI를 숨긴다.
        switch (UIManager.Instance.PopUpUI)
        {
            case POPUP_UI.GAMEOVER:
                UIManager.Instance.InGameUIs.GameOverUI.HideGameResult();
                break;
            case POPUP_UI.LEVEL_SELECTION:
                return;
            case POPUP_UI.OPTION:
                UIManager.Instance.LobbyUIs.OptionUI.Hide();
                break;
        }

        UIManager.Instance.PopUpUI = POPUP_UI.LEVEL_SELECTION;

        animator.Play("Show", -1, 0.0f);
    }

    public void Hide()
    {
        UIManager.Instance.PopUpUI = POPUP_UI.NONE;

        // 애니메이션의 상태를 초기화 해주어야, activeSelf가 애니메이션에 의해 변경되지 않는다.
        animator.Rebind();

        // 다른 UI가 오픈되기 때문에 바로 SetActive로 닫음
        ui.SetActive(false);
    }

    public void OnClickPlayButton()
    {
        Show();
    }

    public void OnClickLevelButton(int controlLevel)
    {
        animator.Play("Hide", -1, 0.0f);

        GameManager.Instance.GameStart((CONTROL_LEVEL)controlLevel);
    }

    public void OnClickExitButton()
    {
        UIManager.Instance.PopUpUI = POPUP_UI.NONE;

        // 애니메이션에 의해 UI를 닫음.
        animator.Play("Hide", -1, 0.0f);

        SoundManager.Instance.PlaySFX("Panel");
    }
}
