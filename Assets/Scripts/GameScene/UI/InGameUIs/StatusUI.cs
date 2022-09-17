using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : UI
{
    // ���� ������ ���� ����
    [SerializeField]
    private GameObject ownedItem;

    // ���̷� ���� ����
    [SerializeField]
    private Siren[] sirens;

    // �÷��� ü�¹� ���� ����
    [SerializeField]
    private GameObject hpBar;

    private float decrementSpeed;

    [SerializeField]
    private Image hpBarFillImage;

    // ������ ���ӽð��� ���� ����
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
        // ��Ȱ��ȭ �� ���̷��� ã�� ���۽�Ų��.
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
