using UnityEngine;

public class CloudScroller : MonoBehaviour
{
    private Vector2 boundHalfSize;
    private float cameraHalfWidth;

    private float speed;

    private void Awake()
    {
        if (TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            boundHalfSize = 0.5f * spriteRenderer.bounds.size;
        }

        cameraHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        speed = Random.Range(0.1f, 0.3f);
    }

    private void Update()
    {
        transform.Translate(speed * Vector2.left * Time.unscaledDeltaTime);
        
        CheckVisibility();
    }

    private void CheckVisibility()
    {
        // ī�޶� ������ �ּ�/�ִ� y��
        float cameraMinY = Camera.main.transform.position.y - Camera.main.orthographicSize;
        float cameraMaxY = Camera.main.transform.position.y + Camera.main.orthographicSize;

        // ���� ������Ʈ�� �ּ�/�ִ� y��
        float minY = transform.position.y - boundHalfSize.y;
        float maxY = transform.position.y + boundHalfSize.y;

        // ���� ������Ʈ�� �ִ� x��
        float maxX = transform.position.x + boundHalfSize.x;

        // ī�޶� ������ ������� �˻��ϰ�, ����ٸ� �ٽ� �����ȿ� ���̵��� ���ġ �Ѵ�.
        if (maxY <= cameraMinY || minY >= cameraMaxY || (maxX <= -cameraHalfWidth))
        {
            // �÷��̾��� y���� 6.0f���϶��, ���� ù ��° ����� ������ ���̹Ƿ� ������ y���� �ּ� 3.0f�̻��� �Ѵ�.
            cameraMinY = (GameManager.Instance.Player.transform.position.y < 6.0f) ? 3.0f : cameraMinY;

            Vector3 newPosition = new Vector3(cameraHalfWidth + boundHalfSize.x, Random.Range(cameraMinY, cameraMaxY), 0.0f);
            float scaleFactor = Random.Range(0.7f, 1.0f);

            transform.position = newPosition;
            transform.localScale = new Vector3(scaleFactor, scaleFactor, 1.0f);
            speed = Random.Range(0.1f, 0.4f);
        }
    }
}
