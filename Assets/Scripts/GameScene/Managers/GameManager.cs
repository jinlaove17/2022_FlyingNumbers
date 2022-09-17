using System.Collections;
using UnityEngine;

public enum CONTROL_LEVEL
{
    EASY   = 2,
    NORMAL = 4,
    HARD   = 6
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private bool isGameStart;

    // 조작 난이도 : 버튼의 개수
    private CONTROL_LEVEL controlLevel;

    // 게임 난이도 : 점수에 비례한 플랫폼의 체력, 장애물/디버프 아이템의 등장 횟수 증가
    private int scoreLevel;             

    private bool isFeverTime;
    private bool canRebirth;

    [SerializeField]
    private Player player;

    [SerializeField]
    private ItemGenerator itemGenerator;
    [SerializeField]
    private ObstacleGenerator obstacleGenerator;

    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    public bool IsGameStart
    {
        get
        {
            return isGameStart;
        }
    }

    public CONTROL_LEVEL ControlLevel
    {
        get
        {
            return controlLevel;
        }
    }

    public int ScoreLevel
    {
        get
        {
            return scoreLevel;
        }

        set
        {
            if (scoreLevel != value && value <= 10)
            {
                scoreLevel = value;

                UIManager.Instance.InGameUIs.StatusUI.DecrementSpeed = 1.0f / (6.2f - 0.5f * scoreLevel);
                obstacleGenerator.GenerateObstacle(10.0f - 0.7f * scoreLevel);
            }
        }
    }

    public bool IsFeverTime
    {
        get
        {
            return isFeverTime;
        }

        set
        {
            isFeverTime = value;
        }
    }

    public bool CanRebirth
    {
        get
        {
            return canRebirth;
        }

        set
        {
            canRebirth = value;

            if (canRebirth)
            {
                UIManager.Instance.InGameUIs.StatusUI.ShowOwnedItem();
            }
            else
            {
                UIManager.Instance.InGameUIs.StatusUI.HideOwnedItem();
            }
        }
    }

    public Player Player
    {
        get
        {
            return player;
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

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Keypad1))
    //    {
    //        UIManager.Instance.InGameUIs.JumpButtonUI.OnClickJumpButton(1);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Keypad2))
    //    {
    //        UIManager.Instance.InGameUIs.JumpButtonUI.OnClickJumpButton(2);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Keypad3))
    //    {
    //        UIManager.Instance.InGameUIs.JumpButtonUI.OnClickJumpButton(3);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Keypad4))
    //    {
    //        UIManager.Instance.InGameUIs.JumpButtonUI.OnClickJumpButton(4);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Keypad5))
    //    {
    //        UIManager.Instance.InGameUIs.JumpButtonUI.OnClickJumpButton(5);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Keypad6))
    //    {
    //        UIManager.Instance.InGameUIs.JumpButtonUI.OnClickJumpButton(6);
    //    }
    //}

    private void OnApplicationPause()
    {
        DataManager.Instance.SaveUserData();
    }

    private void OnApplicationQuit()
    {
        DataManager.Instance.SaveUserData();
    }

    public void GameStart(CONTROL_LEVEL controlLevel)
    {
        isGameStart = true;
        ScoreLevel = 1;
        this.controlLevel = controlLevel;

        StartCoroutine(PrepareToGameStart());

        SoundManager.Instance.PlayBGM("InGame BGM");
    }

    private IEnumerator PrepareToGameStart()
    {
        yield return StartCoroutine(UIManager.Instance.PrepareToGameStart());

        PlatformManager.Instance.PrepareToGameStart();

        player.Init();
        itemGenerator.GenerateItem(10.0f);
        obstacleGenerator.GenerateObstacle(10.0f - 0.7f * scoreLevel);
    }

    public void GameOver()
    {
        isGameStart = false;

        itemGenerator.Stop();
        obstacleGenerator.Stop();

        StartCoroutine(PrepareToGameOver());
    }

    private IEnumerator PrepareToGameOver()
    {
        PlatformManager.Instance.PrepareToGameOver();
        player.Terminate();

        yield return UIManager.Instance.PrepareToGameOver();
    }

    public void EnterFeverTime()
    {
        StartCoroutine(FeverTime());
    }

    private IEnumerator FeverTime()
    {
        itemGenerator.Stop();
        obstacleGenerator.Stop();

        yield return StartCoroutine(PlatformManager.Instance.EnterFeverTime(6.0f));

        itemGenerator.GenerateItem(10.0f);
        obstacleGenerator.GenerateObstacle(10.0f - 0.7f * scoreLevel);
    }
}
