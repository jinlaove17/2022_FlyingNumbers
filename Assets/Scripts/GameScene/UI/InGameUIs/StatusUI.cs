using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : UI
{
    // 소유 아이템 관련 변수
    [SerializeField]
    private GameObject ownedItem;

    // 사이렌 관련 변수
    [SerializeField]
    private Siren[] sirens;

    // 플랫폼 체력바 관련 변수
    [SerializeField]
    private GameObject hpBar;

    private float decrementSpeed;

    [SerializeField]
    private Image hpBarFillImage;

    // 아이템 지속시간바 관련 변수
    [SerializeField]
    private GameObject itemDurationBar;

    [SerializeField]
    private Image itemDurationBarFillImage;

    public float DecrementSpeed
    {
        get
        {
            return decrementSpeed;
        }

        set
        {
            decrementSpeed = value;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameStart)
        {
            if (hpBar.activeSelf)
            {
                switch (GameManager.Instance.Player.State)
                {
                    case PLAYER_STATE.IDLE:
                        hpBarFillImage.fillAmount -= decrementSpeed * Time.deltaTime;

                        if (Mathf.Approximately(hpBarFillImage.fillAmount, 0.0f))
                        {
                            if (PlatformManager.Instance.CurrentPlatform == PlatformManager.Instance.LastPlatform)
                            {
                                DecreaseHpToZero();
                            }
                        }

                        break;
                }
            }
        }
    }

    public override void Init()
    {
        hpBarFillImage.fillAmount = itemDurationBarFillImage.fillAmount = 1.0f;
    }

    public override void Terminate()
    {
        hpBar.SetActive(false);
        itemDurationBar.SetActive(false);

        HideOwnedItem();
    }

    public void ShowOwnedItem()
    {
        animator.Play("ShowOwnedItem", -1, 0.0f);
    }

    public void HideOwnedItem()
    {
        animator.Play("HideOwnedItem", -1, 0.0f);
    }

    public void AlarmSiren(float x)
    {
        // 비활성화 된 사이렌을 찾아 동작시킨다.
        for (int i = 0; i < sirens.Length; ++i)
        {
            if (!sirens[i].gameObject.activeSelf)
            {
                sirens[i].Alarm(x);
                break;
            }
        }
    }

    public void IncreaseHp()
    {
        hpBarFillImage.fillAmount += 25.0f * decrementSpeed * Time.deltaTime;
    }

    public void IncreaseHpToFull()
    {
        hpBarFillImage.fillAmount = 1.0f;
    }

    public void DecreaseHpToZero()
    {
        hpBarFillImage.fillAmount = 0.0f;

        GameManager.Instance.Player.State = PLAYER_STATE.FALLING;
        PlatformManager.Instance.CurrentPlatform = null;
    }

    public IEnumerator ShowItemDurationBar(float duration)
    {
        if (duration > 0.0f)
        {
            itemDurationBarFillImage.fillAmount = 1.0f;
            itemDurationBar.SetActive(true);

            float offset = 1.0f / duration;

            while (itemDurationBarFillImage.fillAmount > 0.0f)
            {
                itemDurationBarFillImage.fillAmount -= offset * Time.deltaTime;

                yield return null;
            }

            itemDurationBarFillImage.fillAmount = 0.0f;
            itemDurationBar.SetActive(false);
        }
    }
}
