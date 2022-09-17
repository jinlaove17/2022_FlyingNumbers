using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    private static PlatformManager instance;

    [SerializeField]
    private Transform platformGroup;

    // 플랫폼 위의 숫자 스프라이트(1 ~ 9)
    [SerializeField]
    private Sprite[] numberSprites;

    // 마지막으로 생성된 플랫폼들
    private Platform[] lastGenPlatforms;

    // 플레이어가 현재 밟고 있는 플랫폼(점프 시, null로 바뀜)
    private Platform currentPlatform;

    // 플레이어가 마지막으로 밟았던 플랫폼(currentPlatform이 null이 되더라도, 다음 플랫폼을 밟기 전까지 유지)
    private Platform lastPlatform;

    // 피버 타임의 영향을 받은 플랫폼들
    private List<Platform> feveredPlatforms;

    // 현재 활성화되어 있는 플랫폼의 개수
    [SerializeField]
    private int platformCount;

    // 현재 플랫폼 생성 시, 갈림길의 수(최대 2)
    private int route;

    public static PlatformManager Instance
    {
        get
        {
            return instance;
        }
    }

    public Sprite[] NumberSprites
    {
        get
        {
            return numberSprites;
        }
    }

    public int PlatformCount
    {
        get
        {
            return platformCount;
        }

        set
        {
            platformCount = value;

            if (platformCount <= 30)
            {
                GeneratePlatform(10);
            }
        }
    }

    public Platform CurrentPlatform
    {
        get
        {
            return currentPlatform;
        }

        set
        {
            currentPlatform = value;

            if (currentPlatform != null)
            {
                lastPlatform = value;
                currentPlatform.HideNumber();

                //UIManager.Instance.InGameUIs.StatusUI.DecreaseHp();
            }
        }
    }

    public Platform LastPlatform
    {
        get
        {
            return lastPlatform;
        }

        // set은 currentPlatform의 프로퍼티에서 수행
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            lastGenPlatforms = new Platform[2];
            feveredPlatforms = new List<Platform>();

            // 씬에 이미 배치된 플랫폼
            ++platformCount;

            route = 1;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PrepareToGameStart()
    {
        // 시작 플랫폼을 생성한다.
        Vector3 position = new Vector3(0.0f, -0.8f, 0.0f);

        Platform newPlatform = PoolingManager.Spawn<Platform>("Platform", position);

        newPlatform.Number = Random.Range(1, (int)GameManager.Instance.ControlLevel + 1);
        newPlatform.NextPlatforms[0] = newPlatform.NextPlatforms[1] = null;

        lastGenPlatforms[0] = lastGenPlatforms[1] = newPlatform;
        platformCount = route = 1;
        currentPlatform = lastPlatform = null;
        feveredPlatforms.Clear();

        GeneratePlatform(19);
    }

    public void PrepareToGameOver()
    {
        Platform[] platforms = platformGroup.GetComponentsInChildren<Platform>();

        foreach (Platform platform in platforms)
        {
            platform.gameObject.SetActive(false);
        }

        platformCount = 0;
    }

    private void GeneratePlatform(int floor)
    {
        // lastGenPlatform[0]이 갱신되기 전, 해당 플랫폼을 저장해둔다.
        Platform prevPlatform = null;

        for (int i = 0; i < floor; ++i)
        {
            // 갈림길을 시작할 것인지에 대한 변수
            bool isSeperated = false;

            // 현재 피버타임이 아니고 경로가 분리되지 않았다면, 10% 확률로 경로를 분리한다.
            if (!GameManager.Instance.IsFeverTime && route == 1)
            {
                if (Random.value < 0.1f)
                {
                    lastGenPlatforms[1] = lastGenPlatforms[0];
                    route = 2;

                    isSeperated = true;
                }
            }

            for (int j = 0; j < route; ++j)
            {
                // 마지막으로 생성된 플랫폼의 인접한 위치에 플랫폼을 생성한다.
                Vector3 position = lastGenPlatforms[j].transform.position;

                position.x += 0.8f * Random.Range(-1, 1 + 1);
                position.y += 0.5f;

                // 플랫폼의 최소/최대 위치를 벗어나는 경우에는 방향을 바꾸어야한다.
                if (position.x < -2.4f)
                {
                    // 가장 좌측에 있으므로, 위 또는 오른쪽 위로 위치를 변경한다.
                    position.x = (Random.value < 0.5f) ? lastGenPlatforms[j].transform.position.x : lastGenPlatforms[j].transform.position.x + 0.8f;
                }
                else if (position.x > 2.4f)
                {
                    // 가장 우측에 있으므로, 위 또는 왼쪽 위로 위치를 변경한다.
                    position.x = (Random.value < 0.5f) ? lastGenPlatforms[j].transform.position.x : lastGenPlatforms[j].transform.position.x - 0.8f;
                }

                // 현재 조작 난이도에 따른 플랫폼의 번호를 선정한다.
                int number = Random.Range(1, (int)GameManager.Instance.ControlLevel + 1);

                Platform newPlatform = null;

                // 첫 번째 플랫폼은 별도의 제약 조건이 없기 때문에 곧바로 생성한다.
                // 이 때, lastGenPlatforms[0]의 값이 newPlatform으로 갱신될 것이기 때문에 갱신전에 prevPlatform에 저장해둔다.
                // prevPlatform은 두 번째 플랫폼을 생성할 때, 첫 번째 플랫폼의 이전 플랫폼과 인접해있는지를 판단하기 위해서 사용한다.
                switch (j)
                {
                    case 0:
                        newPlatform = PoolingManager.Spawn<Platform>("Platform", position);
                        newPlatform.Number = number;
                        newPlatform.NextPlatforms[0] = newPlatform.NextPlatforms[1] = null;

                        if (GameManager.Instance.IsFeverTime)
                        {
                            newPlatform.Number = lastGenPlatforms[0].Number;
                            feveredPlatforms.Add(newPlatform);
                        }

                        prevPlatform = lastGenPlatforms[0];

                        break;
                    case 1:
                        // 두 번째로 생성할 플랫폼의 위치가 첫 번째 생성한 플랫폼의 위치와 같게 되면, 경로를 하나로 합친다.
                        if (Mathf.Approximately(position.x, lastGenPlatforms[0].transform.position.x))
                        {
                            if (!isSeperated)
                            {
                                // lastGenPlatforms[1]은 아직 newPlatform으로 갱신되지 않았으므로, 다음 플랫폼을 갱신한다.
                                lastGenPlatforms[1].NextPlatforms[0] = lastGenPlatforms[0];
                                lastGenPlatforms[1] = lastGenPlatforms[0];
                            }

                            route = 1;

                            continue;
                        }

                        // 위치가 다르다면, 첫 번째로 생성한 플랫폼과 인접해 있는지를 검사한다.
                        float diffX = Mathf.Abs(position.x - lastGenPlatforms[0].transform.position.x);

                        // 인접해 있고 번호가 같다면, 두 번째로 생성한 플랫폼의 번호를 바꿔준다.
                        if (diffX < 1.6f || Mathf.Approximately(diffX, 1.6f))
                        {
                            if (number == lastGenPlatforms[0].Number)
                            {
                                number = number % (int)GameManager.Instance.ControlLevel + 1;
                            }
                        }

                        // 모든 제약 조건을 처리했다면, 새로운 플랫폼을 오브젝트 풀에서 가져온다.
                        newPlatform = PoolingManager.Spawn<Platform>("Platform", position);
                        newPlatform.Number = number;
                        newPlatform.NextPlatforms[0] = newPlatform.NextPlatforms[1] = null;

                        // isSperated가 true인 시점에는 이전 플랫폼이 동일하고, lastGenPlatforms[0]와 [1]이 같기 때문에 아래 로직을 수행하지 않는다.
                        if (!isSeperated)
                        {
                            // 두 번째 생성한 플랫폼과 아까 저장해 둔 이전 플랫폼이 인접해 있는지를 검사한다.
                            diffX = Mathf.Abs(position.x - prevPlatform.transform.position.x);

                            // 인접해 있다면, 이전 플랫폼의 다음 플랫폼으로 두 번째 생성한 플랫폼을 추가한다.
                            if (diffX < 0.8f || Mathf.Approximately(diffX, 0.8f))
                            {
                                prevPlatform.NextPlatforms[1] = newPlatform;
                            }

                            // lastGenPlatforms[1]은 아직 newPlatform으로 갱신되지 않았으므로, prevPlatform과 동일한 층에 있는 나머지 하나의 이전 플랫폼을 가리킨다.
                            // 이 플랫폼과 첫 번째로 생성한 플랫폼이 인접해 있는지를 검사한다.
                            diffX = Mathf.Abs(lastGenPlatforms[0].transform.position.x - lastGenPlatforms[1].transform.position.x);

                            // 인접해 있다면, 이전 플랫폼의 다음 플랫폼으로 첫 번째 생성한 플랫폼을 추가한다.
                            if (diffX < 0.8f || Mathf.Approximately(diffX, 0.8f))
                            {
                                lastGenPlatforms[1].NextPlatforms[1] = lastGenPlatforms[0];
                            }
                        }

                        break;
                }

                if (isSeperated)
                {
                    // 갈림길이 시작되는 플랫폼은 2개의 다음 플랫폼을 갖는다.
                    prevPlatform.NextPlatforms[j] = newPlatform;
                }
                else
                {
                    // 일반적으로 다음 플랫폼은 인덱스 0에 저장된다.
                    lastGenPlatforms[j].NextPlatforms[0] = newPlatform;
                }

                // 플랫폼 생성을 마쳤다면, 마지막으로 생성된 플랫폼을 갱신하고, 활성화 된 플랫폼의 개수를 증가시킨다.
                lastGenPlatforms[j] = newPlatform;
                ++platformCount;
            }
        }
    }

    public IEnumerator EnterFeverTime(float duration)
    {
        // 플레이어가 마지막으로 밟은 플랫폼으로 절대 null이 될 수 없다.
        Platform current = lastPlatform;

        // 경로를 하나로 만든다.
        route = 1;
        lastGenPlatforms[1] = lastGenPlatforms[0];
        current.NextPlatforms[1] = null;

        // 현재 플랫폼부터 마지막으로 생성된 플랫폼까지의 번호를 일치시킨다.
        Platform last = lastGenPlatforms[0];

        // ★ 이 반복문을 수행하는 중에, platform이 GeneratePlatform() 함수에서 사용되어 null이 되는 순간이 겹칠 경우 NullRef 에러가 나는 것 같다.
        for (Platform platform = current; platform != last; platform = platform.NextPlatforms[0])
        {
            platform.Number = last.Number;
            platform.NextPlatforms[1] = null;

            feveredPlatforms.Add(platform);
        }

        // 마지막 플랫폼도 넣어주어야 한다.
        feveredPlatforms.Add(last);

        // 차집합을 이용하여 피버 타임의 영향을 받은 플랫폼을 제외한 모든 플랫폼을 제거한다.
        // 오브젝트 풀링에 의해 플랫폼의 최대 개수가 늘어날 수도 있으므로, 그때마다 GetComponentsInChildren 함수를 호출하는데, 성능 상의 문제를 야기할수도...?
        var restPlatforms = platformGroup.GetComponentsInChildren<Platform>().Except(feveredPlatforms);

        // 하나의 경로를 제외하고 나머지 플랫폼을 없앤다.
        foreach (Platform platform in restPlatforms)
        {
            platform.Die();
        }

        // 피버 타임 여부를 활성화한다.
        GameManager.Instance.IsFeverTime = true;

        // 피버 타임 관련 UI를 출력한다.
        UIManager.Instance.InGameUIs.FeverTimeUI.ShowFeverTimeUI(duration);

        // 피버 타임 배경음으로 변경한다.
        SoundManager.Instance.PlayBGM("FeverTime BGM");

        yield return new WaitForSeconds(duration);

        // 피버 타임의 영향을 받은 플랫폼들의 번호를 다시 재할당한다.
        foreach (Platform platform in feveredPlatforms)
        {
            platform.Number = Random.Range(1, (int)GameManager.Instance.ControlLevel + 1);
        }

        // 피버 타임의 영향을 받은 플랫폼들을 담은 리스트를 초기화한다.
        feveredPlatforms.Clear();

        // 피버 타임을 종료한다.
        GameManager.Instance.IsFeverTime = false;

        // 다시 인게임 배경음으로 변경한다.
        SoundManager.Instance.PlayBGM("InGame BGM");
    }
}
