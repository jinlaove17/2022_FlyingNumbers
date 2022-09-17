using UnityEngine;
using UnityEngine.UI;

public class LobbyUIs : UI
{
    // 0: Easy, 1: Normal, 2: Hard
    [SerializeField]
    private Text[] highScoreTexts;

    [SerializeField]
    private LevelSelectionUI levelSelectionUI;

    [SerializeField]
    private OptionUI optionUI;

    public Text[] HighScoreTexts
    {
        get
        {
            return highScoreTexts;
        }

        set
        {
            highScoreTexts = value;
        }
    }

    public LevelSelectionUI LevelSelectionUI
    {
        get
        {
            return levelSelectionUI;
        }
    }

    public OptionUI OptionUI
    {
        get
        {
            return optionUI;
        }
    }

    public void ShowLobbyUIs()
    {
        animator.Play("Show", -1, 0.0f);
    }

    public void HideLobbyUIs()
    {
        animator.Play("Hide", -1, 0.0f);
    }
}
