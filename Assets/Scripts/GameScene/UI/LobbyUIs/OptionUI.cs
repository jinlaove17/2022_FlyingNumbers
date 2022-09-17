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
        // 패널이 열렸을 때만 퍼센트와 음향을 조절한다.
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
        // 다른 UI를 숨긴다.
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

        // 애니메이션의 상태를 초기화 해주어야, activeSelf가 애니메이션에 의해 변경되지 않는다.
        animator.Rebind();

        // 다른 UI가 오픈되기 때문에 바로 SetActive로 닫음
        ui.SetActive(false);
    }

    public void OnClickOptionButton()
    {
        Show();
    }

    public void OnClickExitButton()
    {
        UIManager.Instance.PopUpUI = POPUP_UI.NONE;

        // 애니메이션에 의해 UI를 닫음.
        animator.Play("Hide", -1, 0.0f);

        SoundManager.Instance.PlaySFX("Panel");
    }
}
