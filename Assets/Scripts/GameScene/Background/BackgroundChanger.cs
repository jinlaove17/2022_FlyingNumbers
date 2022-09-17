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
            // ī�޶� ������ �ּ� y��
            float cameraMinY = Camera.main.transform.position.y - Camera.main.orthographicSize;

            // ���� ����� �ִ� y��
            float backgroundMaxY = backgroundSpriteRenderers[current].transform.position.y + 0.5f * backgroundSpriteRenderers[current].bounds.size.y;

            if (cameraMinY >= backgroundMaxY)
            {
                // ���̻� ���� ����� ���������� �����Ƿ�, ���� ����� ���� ��ġ�� �����Ѵ�.
                backgroundSpriteRenderers[current].transform.position = new Vector3(0.0f, backgroundSpriteRenderers[current].transform.position.y + 2.0f * backgroundSpriteRenderers[current].bounds.size.y);

                // �̶�, �� ����� �̹����� ���� �̹����� �ٲ��ش�.
                backgroundSpriteRenderers[current].sprite = backgroundSprites[scrollCount++];

                if (scrollCount > 7)
                {
                    scrollCount = 7;
                }

                // ����� 2��(�ε��� 0, 1)�� ����ϹǷ� ������ ���� ��ȸ�Ѵ�.
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
