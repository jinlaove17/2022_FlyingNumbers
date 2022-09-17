using UnityEngine;

public class TitleUI : UI
{
    protected override void Awake()
    {
        base.Awake();

        Screen.SetResolution(1080, 1920, true);
        Application.targetFrameRate = 60;
    }

    private void PlaySFX()
    {
        SoundManager.Instance.PlaySFX("Bubble");
    }

    private void PlayBGM()
    {
        SoundManager.Instance.PlayBGM("Title BGM");
    }
}
