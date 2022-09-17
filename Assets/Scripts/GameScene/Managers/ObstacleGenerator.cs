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
            // 플레이어가 마지막으로 밟은 플랫폼으로 절대 null이 될 수 없다.
            Platform currentPlatform = PlatformManager.Instance.LastPlatform;

            // 2 ~ 4개 이후의 플랫폼을 선정한다.
            int nextPlatformCount = Random.Range(2, 4 + 1);

            // 현재 플랫폼부터 선정한 플랫폼까지 x좌표가 같은 플랫폼의 개수이다.
            int duplicationCount = 0;

            // 현재 플랫폼의 x좌표
            float prevX;

            for (int i = 0; i < nextPlatformCount; ++i)
            {
                // 현재 플랫폼이 갈림길인 경우, 둘 중 하나를 선택한다.
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

                // 현재 플랫폼과 다음 플랫폼의 x좌표가 같다면, 중복 카운트를 증가시킨다.
                if (Mathf.Approximately(prevX, currentPlatform.transform.position.x))
                {
                    ++duplicationCount;
                }
                else
                {
                    duplicationCount = 0;
                }
            }

            // 3번 연속 가운데로 점프하는 경로가 아닌 경우에만, 장애물을 생성한다.
            if (duplicationCount < 3)
            {
                float cameraMaxY = Camera.main.transform.position.y + Camera.main.orthographicSize;
                Vector2 position = new Vector2(currentPlatform.transform.position.x, cameraMaxY);

                UIManager.Instance.InGameUIs.StatusUI.AlarmSiren(position.x);

                // 사이렌을 울린 뒤, sirenAlarmTime초 후에 생성한다.
                yield return sirenAlarmTime;

                PoolingManager.Spawn("Obstacle", position);

                yield return genTime;
            }
        }
    }
}
