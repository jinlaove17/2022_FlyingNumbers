using UnityEngine;

public class LevelSelectionUI : UI
{
    [SerializeField]
    private GameObject ui;

    private void Show()
    {
        // �ٸ� UI�� �����.
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

        // �ִϸ��̼��� ���¸� �ʱ�ȭ ���־��, activeSelf�� �ִϸ��̼ǿ� ���� ������� �ʴ´�.
        animator.Rebind();

        // �ٸ� UI�� ���µǱ� ������ �ٷ� SetActive�� ����
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

        // �ִϸ��̼ǿ� ���� UI�� ����.
        animator.Play("Hide", -1, 0.0f);

        SoundManager.Instance.PlaySFX("Panel");
    }
}
