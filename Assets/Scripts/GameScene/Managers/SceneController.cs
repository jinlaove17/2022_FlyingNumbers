using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    private static SceneController instance;

    [SerializeField]
    private GameObject fadePrefab;

    [SerializeField]
    private GameObject currentFadeBackground;

    public static SceneController Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        DataManager.Instance.ApplyUserData();

        StartCoroutine(FadeIn());
    }

    public void MoveToScene(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    private IEnumerator FadeOut(string nextSceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(nextSceneName);

        // ���� ��ȯ���� �ʵ���, �ε��� 90%������ �����Ѵ�.
        asyncOperation.allowSceneActivation = false;

        if (currentFadeBackground == null)
        {
            currentFadeBackground = Instantiate(fadePrefab, FindObjectOfType<Canvas>().transform);
        }
        else
        {
            currentFadeBackground.SetActive(true);
        }

        Image fadeImage = currentFadeBackground.GetComponent<Image>();
        Color color = Vector4.zero;

        fadeImage.color = color;

        while (fadeImage.color.a < 1.0f)
        {
            color.a += 0.8f * Time.unscaledDeltaTime;
            fadeImage.color = color;

            SoundManager.Instance.BGMVolume = 0.3f * (1.0f - color.a);

            yield return null;
        }

        asyncOperation.allowSceneActivation = true;

        switch (nextSceneName)
        {
            case "GameScene":
                SoundManager.Instance.PlayBGM("Lobby BGM");
                break;
        }
    }

    private IEnumerator FadeIn()
    {
        if (currentFadeBackground == null)
        {
            currentFadeBackground = Instantiate(fadePrefab, FindObjectOfType<Canvas>().transform);
        }

        Image fadeImage = currentFadeBackground.GetComponent<Image>();
        Color color = Color.black;

        fadeImage.color = color;

        while (fadeImage.color.a > 0.0f)
        {
            color.a -= 0.8f * Time.unscaledDeltaTime;
            fadeImage.color = color;

            SoundManager.Instance.BGMVolume = 0.3f * (1.0f - color.a);

            yield return null;
        }

        SoundManager.Instance.BGMVolume = 0.3f;

        // �ٸ� UI���� Ŭ������ �ʱ� ������ ���־�� �Ѵ�.
        currentFadeBackground.SetActive(false);
    }
}
