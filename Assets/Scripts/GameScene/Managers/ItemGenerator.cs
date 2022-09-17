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
            // �÷��̾ ���������� ���� �÷������� ���� null�� �� �� ����.
            Platform currentPlatform = PlatformManager.Instance.LastPlatform;

            // 2 ~ 4�� ������ �÷����� �����Ѵ�.
            int nextPlatformCount = Random.Range(2, 4 + 1);

            for (int i = 0; i < nextPlatformCount; ++i)
            {
                // ���� �÷����� �������� ���, �� �� �ϳ��� �����Ѵ�.
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

            // ���̷��� �︰ ��, sirenAlarmTime�� �Ŀ� �����Ѵ�.
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
