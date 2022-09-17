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
        // 카메라 영역의 최소/최대 y값
        float cameraMinY = Camera.main.transform.position.y - Camera.main.orthographicSize;
        float cameraMaxY = Camera.main.transform.position.y + Camera.main.orthographicSize;

        // 현재 오브젝트의 최소/최대 y값
        float minY = transform.position.y - boundHalfSize.y;
        float maxY = transform.position.y + boundHalfSize.y;

        // 현재 오브젝트의 최대 x값
        float maxX = transform.position.x + boundHalfSize.x;

        // 카메라 영역을 벗어났는지 검사하고, 벗어났다면 다시 영역안에 보이도록 재배치 한다.
        if (maxY <= cameraMinY || minY >= cameraMaxY || (maxX <= -cameraHalfWidth))
        {
            // 플레이어의 y값이 6.0f이하라면, 아직 첫 번째 배경이 렌더링 중이므로 구름의 y값은 최소 3.0f이상어야 한다.
            cameraMinY = (GameManager.Instance.Player.transform.position.y < 6.0f) ? 3.0f : cameraMinY;

            Vector3 newPosition = new Vector3(cameraHalfWidth + boundHalfSize.x, Random.Range(cameraMinY, cameraMaxY), 0.0f);
            float scaleFactor = Random.Range(0.7f, 1.0f);

            transform.position = newPosition;
            transform.localScale = new Vector3(scaleFactor, scaleFactor, 1.0f);
            speed = Random.Range(0.1f, 0.4f);
        }
    }
}
