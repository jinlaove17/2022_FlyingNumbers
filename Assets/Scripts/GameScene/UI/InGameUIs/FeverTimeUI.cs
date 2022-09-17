using UnityEngine;

public class FeverTimeUI : UI
{
    [SerializeField]
    private BackgroundChanger backgroundChanger;

    public override void Terminate()
    {
        backgroundChanger.Terminate();
    }

    public void ShowFeverTimeUI(float duration)
    {
        StartCoroutine(backgroundChanger.ShowFeverTimeBackground(duration));
        StartCoroutine(UIManager.Instance.InGameUIs.StatusUI.ShowItemDurationBar(duration));

        animator.Play("Show", -1, 0.0f);
    }
}
