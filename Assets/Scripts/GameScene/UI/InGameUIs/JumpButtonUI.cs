using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class JumpButtonUI : UI
{
    [SerializeField]
    private RectTransform[] buttonRectTransforms;

    public override void Init()
    {
        // ������ ���� ���̵��� ���� ��ư�� ��ġ�Ѵ�.
        int controlLevel = (int)GameManager.Instance.ControlLevel;

        for (int i = 0; i < buttonRectTransforms.Length; i += 2)
        {
            if (i < controlLevel)
            {
                float y = 130.0f + 200.0f * ((controlLevel / 2 - 1) - i / 2);

                buttonRectTransforms[i].anchoredPosition = new Vector3(buttonRectTransforms[i].anchoredPosition.x, y, 0.0f);
                buttonRectTransforms[i + 1].anchoredPosition = new Vector3(buttonRectTransforms[i + 1].anchoredPosition.x, y, 0.0f);
                buttonRectTransforms[i].gameObject.SetActive(true);
                buttonRectTransforms[i + 1].gameObject.SetActive(true);
            }
            else
            {
                buttonRectTransforms[i].gameObject.SetActive(false);
                buttonRectTransforms[i + 1].gameObject.SetActive(false);
            }
        }
    }

    public void ShuffleButton()
    {
        StartCoroutine(Shuffle(6.0f));
    }

    private IEnumerator Shuffle(float duration)
    {
        int controlLevel = (int)GameManager.Instance.ControlLevel;
        int[] originOrder = new int[controlLevel];
        int[] newOrder = new int[controlLevel];

        for (int i = 0; i < controlLevel; ++i)
        {
            originOrder[i] = newOrder[i] = i;
        }

        // ���� ����(0, 1, 2,...)�� ������ ���� ������ �ε����� �����ش�.
        System.Random random = new System.Random();

        while (Enumerable.SequenceEqual(originOrder, newOrder))
        {
            newOrder = newOrder.OrderBy(i => random.Next()).ToArray();
        }

        Vector2[] originPositions = new Vector2[controlLevel];

        for (int i = 0; i < controlLevel; ++i)
        {
            originPositions[i] = buttonRectTransforms[i].anchoredPosition;
        }

        for (int i = 0; i < controlLevel; ++i)
        {
            buttonRectTransforms[i].anchoredPosition = originPositions[newOrder[i]];
        }

        StartCoroutine(UIManager.Instance.InGameUIs.StatusUI.ShowItemDurationBar(duration));

        // duration�� ���� ��ư�� �迭�� �ٲ۴�.
        yield return new WaitForSeconds(duration);

        // duration�� �Ŀ��� ���� �迭�� �ǵ�����.
        for (int i = 0; i < controlLevel; ++i)
        {
            buttonRectTransforms[i].anchoredPosition = originPositions[i];
        }
    }

    public void OnClickJumpButton(int buttonNumber)
    {
        if (GameManager.Instance.Player.State == PLAYER_STATE.IDLE)
        {
            Platform currentPlatform = PlatformManager.Instance.CurrentPlatform;

            if (currentPlatform != null)
            {
                Platform nextPlatform = Array.Find(currentPlatform.NextPlatforms,
                    (Platform platform) =>
                    {
                        if (platform != null)
                            return platform.Number == buttonNumber;
                        else
                            return false;
                    }
                    );

                // �ش� ��ȣ�� ���� �÷����� ���� �� ���ٸ�, �߸����� ���̹Ƿ� �÷��̾ ����߸���.
                if (nextPlatform == null)
                {
                    UIManager.Instance.InGameUIs.StatusUI.DecreaseHpToZero();
                }
                else
                {
                    Vector3 toNextPlatform = nextPlatform.transform.position - currentPlatform.transform.position;

                    GameManager.Instance.Player.Jump(toNextPlatform.normalized);
                }
            }
        }
    }
}
