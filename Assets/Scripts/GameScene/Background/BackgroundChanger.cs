using System.Collections;
using UnityEngine;

public class BackgroundChanger : MonoBehaviour
{
    private Animator animator;

    [SerializeField]
    private SpriteRenderer[] backgroundSpriteRenderers;

    [SerializeField]
    private Sprite[] backgroundSprites;

    private int current;
    private int scrollCount;

    [SerializeField]
    private GameObject feverTimeBackground;

    private float pivotDiffY;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        scrollCount = 2;

        if (feverTimeBackground.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            pivotDiffY = Mathf.Abs(0.5f * spriteRenderer.bounds.size.y - Camera.main.orthographicSize);
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameStart)
        {
            // 카메라 영역의 최소 y값
            float cameraMinY = Camera.main.transform.position.y - Camera.main.orthographicSize;

            // 현재 배경의 최대 y값
            float backgroundMaxY = backgroundSpriteRenderers[current].transform.position.y + 0.5f * backgroundSpriteRenderers[current].bounds.size.y;

            if (cameraMinY >= backgroundMaxY)
            {
                // 더이상 현재 배경은 렌더링되지 않으므로, 다음 배경의 다음 위치로 설정한다.
                backgroundSpriteRenderers[current].transform.position = new Vector3(0.0f, backgroundSpriteRenderers[current].transform.position.y + 2.0f * backgroundSpriteRenderers[current].bounds.size.y);

                // 이때, 뒷 배경의 이미지도 다음 이미지로 바꿔준다.
                backgroundSpriteRenderers[current].sprite = backgroundSprites[scrollCount++];

                if (scrollCount > 7)
                {
                    scrollCount = 7;
                }

                // 배경은 2개(인덱스 0, 1)을 사용하므로 다음과 같이 순회한다.
                current = (current + 1) % backgroundSpriteRenderers.Length;
            }
        }
    }

    public void Init()
    {

    }

    public void Terminate()
    {
        current = 0;
        scrollCount = 2;

        backgroundSpriteRenderers[current].transform.position = new Vector3(0.0f, 1.2f, 0.0f);
        backgroundSpriteRenderers[current].sprite = backgroundSprites[0];
        backgroundSpriteRenderers[current + 1].transform.position = new Vector3(0.0f, backgroundSpriteRenderers[current].transform.position.y + backgroundSpriteRenderers[current].bounds.size.y);
        backgroundSpriteRenderers[current + 1].sprite = backgroundSprites[1];

        feverTimeBackground.SetActive(false);
        feverTimeBackground.transform.position = Vector3.zero;
    }

    public IEnumerator ShowFeverTimeBackground(float duration)
    {
        if (duration > 0.0f)
        {
            Vector2 newPosition = new Vector2(0.0f, Camera.main.transform.position.y + pivotDiffY);

            feverTimeBackground.transform.position = newPosition;
            feverTimeBackground.SetActive(true);
            animator.Play("Show", -1, 0.0f);

            while (true)
            {
                newPosition.y -= 0.5f * Time.deltaTime;

                if (feverTimeBackground.transform.position.y - Camera.main.transform.position.y <= -pivotDiffY)
                {
                    break;
                }

                feverTimeBackground.transform.position = newPosition;

                yield return null;
            }

            feverTimeBackground.SetActive(false);
        }
    }
}
