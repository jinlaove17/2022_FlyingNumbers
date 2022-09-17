using UnityEngine;
using UnityEngine.UI;

public class OptionUI : UI
{
    [SerializeField]
    private GameObject ui;

    [SerializeField]
    private Slider bgmBar;

    [SerializeField]
    private Slider sfxBar;

    [SerializeField]
    private Text bgmPerText;

    [SerializeField]
    private Text sfxPerText;

    private void Update()
    {
        // �г��� ������ ���� �ۼ�Ʈ�� ������ �����Ѵ�.
        if (ui.activeSelf)
        {
            bgmPerText.text = (int)(100.0f * bgmBar.value) + "%";
            sfxPerText.text = (int)(100.0f * sfxBar.value) + "%";

            SoundManager.Instance.BGMVolume = bgmBar.value;
            SoundManager.Instance.SFXVolume = sfxBar.value;
        }
    }

    private void Show()
    {
        // �ٸ� UI�� �����.
        switch (UIManager.Instance.PopUpUI)
        {
            case POPUP_UI.GAMEOVER:
                UIManager.Instance.InGameUIs.GameOverUI.HideGameResult();
                break;
            case POPUP_UI.LEVEL_SELECTION:
                UIManager.Instance.LobbyUIs.LevelSelectionUI.Hide();
                break;
            case POPUP_UI.OPTION:
                return;
        }

        UIManager.Instance.PopUpUI = POPUP_UI.OPTION;

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

    public void OnClickOptionButton()
    {
        Show();
    }

    public void OnClickExitButton()
    {
        UIManager.Instance.PopUpUI = POPUP_UI.NONE;

        // �ִϸ��̼ǿ� ���� UI�� ����.
        animator.Play("Hide", -1, 0.0f);

        SoundManager.Instance.PlaySFX("Panel");
    }
}
