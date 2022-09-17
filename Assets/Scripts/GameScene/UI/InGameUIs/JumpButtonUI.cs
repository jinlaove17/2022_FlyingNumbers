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
        // 설정된 조작 난이도에 따라 버튼을 배치한다.
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

        // 원래 순서(0, 1, 2,...)가 나오지 않을 때까지 인덱스를 섞어준다.
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

        // duration초 동안 버튼의 배열을 바꾼다.
        yield return new WaitForSeconds(duration);

        // duration초 후에는 원래 배열로 되돌린다.
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

                // 해당 번호를 가진 플랫폼을 가질 수 없다면, 잘못누른 것이므로 플레이어를 떨어뜨린다.
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
