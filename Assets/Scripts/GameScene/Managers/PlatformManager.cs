using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    private static PlatformManager instance;

    [SerializeField]
    private Transform platformGroup;

    // �÷��� ���� ���� ��������Ʈ(1 ~ 9)
    [SerializeField]
    private Sprite[] numberSprites;

    // ���������� ������ �÷�����
    private Platform[] lastGenPlatforms;

    // �÷��̾ ���� ��� �ִ� �÷���(���� ��, null�� �ٲ�)
    private Platform currentPlatform;

    // �÷��̾ ���������� ��Ҵ� �÷���(currentPlatform�� null�� �Ǵ���, ���� �÷����� ��� ������ ����)
    private Platform lastPlatform;

    // �ǹ� Ÿ���� ������ ���� �÷�����
    private List<Platform> feveredPlatforms;

    // ���� Ȱ��ȭ�Ǿ� �ִ� �÷����� ����
    [SerializeField]
    private int platformCount;

    // ���� �÷��� ���� ��, �������� ��(�ִ� 2)
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

        // set�� currentPlatform�� ������Ƽ���� ����
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            lastGenPlatforms = new Platform[2];
            feveredPlatforms = new List<Platform>();

            // ���� �̹� ��ġ�� �÷���
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
        // ���� �÷����� �����Ѵ�.
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
        // lastGenPlatform[0]�� ���ŵǱ� ��, �ش� �÷����� �����صд�.
        Platform prevPlatform = null;

        for (int i = 0; i < floor; ++i)
        {
            // �������� ������ �������� ���� ����
            bool isSeperated = false;

            // ���� �ǹ�Ÿ���� �ƴϰ� ��ΰ� �и����� �ʾҴٸ�, 10% Ȯ���� ��θ� �и��Ѵ�.
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
                // ���������� ������ �÷����� ������ ��ġ�� �÷����� �����Ѵ�.
                Vector3 position = lastGenPlatforms[j].transform.position;

                position.x += 0.8f * Random.Range(-1, 1 + 1);
                position.y += 0.5f;

                // �÷����� �ּ�/�ִ� ��ġ�� ����� ��쿡�� ������ �ٲپ���Ѵ�.
                if (position.x < -2.4f)
                {
                    // ���� ������ �����Ƿ�, �� �Ǵ� ������ ���� ��ġ�� �����Ѵ�.
                    position.x = (Random.value < 0.5f) ? lastGenPlatforms[j].transform.position.x : lastGenPlatforms[j].transform.position.x + 0.8f;
                }
                else if (position.x > 2.4f)
                {
                    // ���� ������ �����Ƿ�, �� �Ǵ� ���� ���� ��ġ�� �����Ѵ�.
                    position.x = (Random.value < 0.5f) ? lastGenPlatforms[j].transform.position.x : lastGenPlatforms[j].transform.position.x - 0.8f;
                }

                // ���� ���� ���̵��� ���� �÷����� ��ȣ�� �����Ѵ�.
                int number = Random.Range(1, (int)GameManager.Instance.ControlLevel + 1);

                Platform newPlatform = null;

                // ù ��° �÷����� ������ ���� ������ ���� ������ ��ٷ� �����Ѵ�.
                // �� ��, lastGenPlatforms[0]�� ���� newPlatform���� ���ŵ� ���̱� ������ �������� prevPlatform�� �����صд�.
                // prevPlatform�� �� ��° �÷����� ������ ��, ù ��° �÷����� ���� �÷����� �������ִ����� �Ǵ��ϱ� ���ؼ� ����Ѵ�.
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
                        // �� ��°�� ������ �÷����� ��ġ�� ù ��° ������ �÷����� ��ġ�� ���� �Ǹ�, ��θ� �ϳ��� ��ģ��.
                        if (Mathf.Approximately(position.x, lastGenPlatforms[0].transform.position.x))
                        {
                            if (!isSeperated)
                            {
                                // lastGenPlatforms[1]�� ���� newPlatform���� ���ŵ��� �ʾ����Ƿ�, ���� �÷����� �����Ѵ�.
                                lastGenPlatforms[1].NextPlatforms[0] = lastGenPlatforms[0];
                                lastGenPlatforms[1] = lastGenPlatforms[0];
                            }

                            route = 1;

                            continue;
                        }

                        // ��ġ�� �ٸ��ٸ�, ù ��°�� ������ �÷����� ������ �ִ����� �˻��Ѵ�.
                        float diffX = Mathf.Abs(position.x - lastGenPlatforms[0].transform.position.x);

                        // ������ �ְ� ��ȣ�� ���ٸ�, �� ��°�� ������ �÷����� ��ȣ�� �ٲ��ش�.
                        if (diffX < 1.6f || Mathf.Approximately(diffX, 1.6f))
                        {
                            if (number == lastGenPlatforms[0].Number)
                            {
                                number = number % (int)GameManager.Instance.ControlLevel + 1;
                            }
                        }

                        // ��� ���� ������ ó���ߴٸ�, ���ο� �÷����� ������Ʈ Ǯ���� �����´�.
                        newPlatform = PoolingManager.Spawn<Platform>("Platform", position);
                        newPlatform.Number = number;
                        newPlatform.NextPlatforms[0] = newPlatform.NextPlatforms[1] = null;

                        // isSperated�� true�� �������� ���� �÷����� �����ϰ�, lastGenPlatforms[0]�� [1]�� ���� ������ �Ʒ� ������ �������� �ʴ´�.
                        if (!isSeperated)
                        {
                            // �� ��° ������ �÷����� �Ʊ� ������ �� ���� �÷����� ������ �ִ����� �˻��Ѵ�.
                            diffX = Mathf.Abs(position.x - prevPlatform.transform.position.x);

                            // ������ �ִٸ�, ���� �÷����� ���� �÷������� �� ��° ������ �÷����� �߰��Ѵ�.
                            if (diffX < 0.8f || Mathf.Approximately(diffX, 0.8f))
                            {
                                prevPlatform.NextPlatforms[1] = newPlatform;
                            }

                            // lastGenPlatforms[1]�� ���� newPlatform���� ���ŵ��� �ʾ����Ƿ�, prevPlatform�� ������ ���� �ִ� ������ �ϳ��� ���� �÷����� ����Ų��.
                            // �� �÷����� ù ��°�� ������ �÷����� ������ �ִ����� �˻��Ѵ�.
                            diffX = Mathf.Abs(lastGenPlatforms[0].transform.position.x - lastGenPlatforms[1].transform.position.x);

                            // ������ �ִٸ�, ���� �÷����� ���� �÷������� ù ��° ������ �÷����� �߰��Ѵ�.
                            if (diffX < 0.8f || Mathf.Approximately(diffX, 0.8f))
                            {
                                lastGenPlatforms[1].NextPlatforms[1] = lastGenPlatforms[0];
                            }
                        }

                        break;
                }

                if (isSeperated)
                {
                    // �������� ���۵Ǵ� �÷����� 2���� ���� �÷����� ���´�.
                    prevPlatform.NextPlatforms[j] = newPlatform;
                }
                else
                {
                    // �Ϲ������� ���� �÷����� �ε��� 0�� ����ȴ�.
                    lastGenPlatforms[j].NextPlatforms[0] = newPlatform;
                }

                // �÷��� ������ ���ƴٸ�, ���������� ������ �÷����� �����ϰ�, Ȱ��ȭ �� �÷����� ������ ������Ų��.
                lastGenPlatforms[j] = newPlatform;
                ++platformCount;
            }
        }
    }

    public IEnumerator EnterFeverTime(float duration)
    {
        // �÷��̾ ���������� ���� �÷������� ���� null�� �� �� ����.
        Platform current = lastPlatform;

        // ��θ� �ϳ��� �����.
        route = 1;
        lastGenPlatforms[1] = lastGenPlatforms[0];
        current.NextPlatforms[1] = null;

        // ���� �÷������� ���������� ������ �÷��������� ��ȣ�� ��ġ��Ų��.
        Platform last = lastGenPlatforms[0];

        // �� �� �ݺ����� �����ϴ� �߿�, platform�� GeneratePlatform() �Լ����� ���Ǿ� null�� �Ǵ� ������ ��ĥ ��� NullRef ������ ���� �� ����.
        for (Platform platform = current; platform != last; platform = platform.NextPlatforms[0])
        {
            platform.Number = last.Number;
            platform.NextPlatforms[1] = null;

            feveredPlatforms.Add(platform);
        }

        // ������ �÷����� �־��־�� �Ѵ�.
        feveredPlatforms.Add(last);

        // �������� �̿��Ͽ� �ǹ� Ÿ���� ������ ���� �÷����� ������ ��� �÷����� �����Ѵ�.
        // ������Ʈ Ǯ���� ���� �÷����� �ִ� ������ �þ ���� �����Ƿ�, �׶����� GetComponentsInChildren �Լ��� ȣ���ϴµ�, ���� ���� ������ �߱��Ҽ���...?
        var restPlatforms = platformGroup.GetComponentsInChildren<Platform>().Except(feveredPlatforms);

        // �ϳ��� ��θ� �����ϰ� ������ �÷����� ���ش�.
        foreach (Platform platform in restPlatforms)
        {
            platform.Die();
        }

        // �ǹ� Ÿ�� ���θ� Ȱ��ȭ�Ѵ�.
        GameManager.Instance.IsFeverTime = true;

        // �ǹ� Ÿ�� ���� UI�� ����Ѵ�.
        UIManager.Instance.InGameUIs.FeverTimeUI.ShowFeverTimeUI(duration);

        // �ǹ� Ÿ�� ��������� �����Ѵ�.
        SoundManager.Instance.PlayBGM("FeverTime BGM");

        yield return new WaitForSeconds(duration);

        // �ǹ� Ÿ���� ������ ���� �÷������� ��ȣ�� �ٽ� ���Ҵ��Ѵ�.
        foreach (Platform platform in feveredPlatforms)
        {
            platform.Number = Random.Range(1, (int)GameManager.Instance.ControlLevel + 1);
        }

        // �ǹ� Ÿ���� ������ ���� �÷������� ���� ����Ʈ�� �ʱ�ȭ�Ѵ�.
        feveredPlatforms.Clear();

        // �ǹ� Ÿ���� �����Ѵ�.
        GameManager.Instance.IsFeverTime = false;

        // �ٽ� �ΰ��� ��������� �����Ѵ�.
        SoundManager.Instance.PlayBGM("InGame BGM");
    }
}
