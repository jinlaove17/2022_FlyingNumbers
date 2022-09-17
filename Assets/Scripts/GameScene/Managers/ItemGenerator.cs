using System.Collections;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    [SerializeField]
    private Transform itemGroup;

    private Coroutine itemGenCoroutine;

    public void GenerateItem(float genPeriod)
    {
        Stop();

        itemGenCoroutine = StartCoroutine(Generate(genPeriod));
    }

    public void Stop()
    {
        if (itemGenCoroutine != null)
        {
            StopCoroutine(itemGenCoroutine);
            InactiveAllItem();

            itemGenCoroutine = null;
        }
    }

    private void InactiveAllItem()
    {
        for (int i = 0; i < itemGroup.childCount; ++i)
        {
            itemGroup.GetChild(i).gameObject.SetActive(false);
        }
    }

    private IEnumerator Generate(float genPeriod)
    {
        WaitForSeconds sirenAlarmTime = new WaitForSeconds(1.0f);
        WaitForSeconds genTime = new WaitForSeconds(genPeriod);

        yield return genTime;

        while (true)
        {
            // 플레이어가 마지막으로 밟은 플랫폼으로 절대 null이 될 수 없다.
            Platform currentPlatform = PlatformManager.Instance.LastPlatform;

            // 2 ~ 4개 이후의 플랫폼을 선정한다.
            int nextPlatformCount = Random.Range(2, 4 + 1);

            for (int i = 0; i < nextPlatformCount; ++i)
            {
                // 현재 플랫폼이 갈림길인 경우, 둘 중 하나를 선택한다.
                if (currentPlatform.NextPlatforms[1] != null)
                {
                    currentPlatform = currentPlatform.NextPlatforms[Random.Range(0, 1 + 1)];
                }
                else
                {
                    currentPlatform = currentPlatform.NextPlatforms[0];
                }
            }

            float cameraMaxY = Camera.main.transform.position.y + Camera.main.orthographicSize;
            Vector2 genPosition = new Vector2(currentPlatform.transform.position.x, cameraMaxY);

            UIManager.Instance.InGameUIs.StatusUI.AlarmSiren(genPosition.x);

            // 사이렌을 울린 뒤, sirenAlarmTime초 후에 생성한다.
            yield return sirenAlarmTime;

            int probability = Random.Range(1, 100 + 1);

            if (probability <= 10)
            {
                PoolingManager.Spawn("RevivalItem", genPosition);
            }
            else if (probability <= 40)
            {
                PoolingManager.Spawn("FeverItem", genPosition);
            }
            else
            {
                PoolingManager.Spawn("ButtonChangeItem", genPosition);
            }

            yield return genTime;
        }
    }
}
