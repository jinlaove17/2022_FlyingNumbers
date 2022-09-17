using UnityEngine;

public class InGameUIs : UI
{
    [SerializeField]
    private ScoreUI scoreUI;

    [SerializeField]
    private StatusUI statusUI;

    [SerializeField]
    private JumpButtonUI jumpButtonUI;

    [SerializeField]
    private FeverTimeUI feverTimeUI;

    [SerializeField]
    private GameOverUI gameOverUI;

    public ScoreUI ScoreUI
    { 
        get
        { 
            return scoreUI;
        }
    }

    public StatusUI StatusUI
    {
        get
        {
            return statusUI;
        }
    }

    public JumpButtonUI JumpButtonUI
    {
        get
        {
            return jumpButtonUI;
        }
    }

    public FeverTimeUI FeverTimeUI
    {
        get
        {
            return feverTimeUI;
        }
    }

    public GameOverUI GameOverUI
    {
        get
        {
            return gameOverUI;
        }
    }

    public override void Init()
    {
        scoreUI.Init();
        statusUI.Init();
        jumpButtonUI.Init();
        feverTimeUI.Init();
        gameOverUI.Init();
    }

    public override void Terminate()
    {
        scoreUI.Terminate();
        statusUI.Terminate();
        jumpButtonUI.Terminate();
        feverTimeUI.Terminate();
        gameOverUI.Terminate();
    }

    public void ShowInGameUIs()
    {
        animator.Play("Show", -1, 0.0f);
    }

    public void HideInGameUIs()
    {
        animator.Play("Hide", -1, 0.0f);
    }
}
