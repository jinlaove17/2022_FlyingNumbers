using System.Collections;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    [SerializeField]
    private Transform obstacleGroup;

    private Coroutine obstacleGenCoroutine;

    public void GenerateObstacle(float genPeriod)
    {
        Stop();

        obstacleGenCoroutine = StartCoroutine(Generate(genPeriod));
    }

    public void Stop()
    {
        if (obstacleGenCoroutine != null)
        {
            StopCoroutine(obstacleGenCoroutine);
            InactiveAllObstacle();

            obstacleGenCoroutine = null;
        }
    }

    private void InactiveAllObstacle()
    {
        for (int i = 0; i < obstacleGroup.childCount; ++i)
        {
            obstacleGroup.GetChild(i).gameObject.SetActive(false);
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

            // ���� �÷������� ������ �÷������� x��ǥ�� ���� �÷����� �����̴�.
            int duplicationCount = 0;

            // ���� �÷����� x��ǥ
            float prevX;

            for (int i = 0; i < nextPlatformCount; ++i)
            {
                // ���� �÷����� �������� ���, �� �� �ϳ��� �����Ѵ�.
                if (currentPlatform.NextPlatforms[1] != null)
                {
                    prevX = currentPlatform.transform.position.x;
                    currentPlatform = currentPlatform.NextPlatforms[Random.Range(0, 1 + 1)];
                }
                else
                {
                    prevX = currentPlatform.transform.position.x;
                    currentPlatform = currentPlatform.NextPlatforms[0];
                }

                // ���� �÷����� ���� �÷����� x��ǥ�� ���ٸ�, �ߺ� ī��Ʈ�� ������Ų��.
                if (Mathf.Approximately(prevX, currentPlatform.transform.position.x))
                {
                    ++duplicationCount;
                }
                else
                {
                    duplicationCount = 0;
                }
            }

            // 3�� ���� ����� �����ϴ� ��ΰ� �ƴ� ��쿡��, ��ֹ��� �����Ѵ�.
            if (duplicationCount < 3)
            {
                float cameraMaxY = Camera.main.transform.position.y + Camera.main.orthographicSize;
                Vector2 position = new Vector2(currentPlatform.transform.position.x, cameraMaxY);

                UIManager.Instance.InGameUIs.StatusUI.AlarmSiren(position.x);

                // ���̷��� �︰ ��, sirenAlarmTime�� �Ŀ� �����Ѵ�.
                yield return sirenAlarmTime;

                PoolingManager.Spawn("Obstacle", position);

                yield return genTime;
            }
        }
    }
}
