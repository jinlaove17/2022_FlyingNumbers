using System.Collections;
using UnityEngine;

public enum POPUP_UI
{
    NONE,
    GAMEOVER,
    LEVEL_SELECTION,
    OPTION
}

public class UIManager : MonoBehaviour
{
    static private UIManager instance;

    [SerializeField]
    private LobbyUIs lobbyUIs;

    [SerializeField]
    private InGameUIs inGameUIs;

    private POPUP_UI popupUI;

    public static UIManager Instance
    {
        get
        {
            return instance;
        }
    }

    public LobbyUIs LobbyUIs
    {
        get
        {
            return lobbyUIs;
        }
    }

    public InGameUIs InGameUIs
    {
        get
        {
            return inGameUIs;
        }
    }

    public POPUP_UI PopUpUI
    {
        get
        {
            return popupUI;
        }

        set
        {
            popupUI = value;
        }
    }
     
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        lobbyUIs.ShowLobbyUIs();
    }

    public IEnumerator PrepareToGameStart()
    {
        inGameUIs.Init();
        lobbyUIs.HideLobbyUIs();

        yield return new WaitForSeconds(0.8f);

        inGameUIs.ShowInGameUIs();
    }

    public IEnumerator PrepareToGameOver()
    {
        inGameUIs.Terminate();
        inGameUIs.HideInGameUIs();
        inGameUIs.GameOverUI.ShowGameResult();

        SoundManager.Instance.PlaySFX("GameOver");

        yield return new WaitForSeconds(0.8f);

        lobbyUIs.ShowLobbyUIs();

        SoundManager.Instance.PlayBGM("Lobby BGM");
    }

    // 버튼의 Event Trigger 컴포넌트에서 사용될 함수들이다.
    public void PlayButtonSFX()
    {
        SoundManager.Instance.PlaySFX("Button");
    }

    public void PlayPanelSFX()
    {
        SoundManager.Instance.PlaySFX("Panel");
    }
}
