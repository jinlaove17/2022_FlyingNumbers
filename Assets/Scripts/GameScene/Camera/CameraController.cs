using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private float speed;

    [SerializeField]
    private Vector2 center;

    [SerializeField]
    private Vector2 maxSize;

    private float offset;
    private Vector2 size;

    private void Awake()
    {
        offset = 1.0f;
        size = new Vector2((Camera.main.orthographicSize * Screen.width) / Screen.height, Camera.main.orthographicSize);
    }

    private void Start()
    {
        target = GameManager.Instance.Player.transform;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, speed * Time.deltaTime);

        if (GameManager.Instance.Player.State == PLAYER_STATE.IDLE || GameManager.Instance.Player.State == PLAYER_STATE.JUMPING)
        {
            center = new Vector2(0.0f, target.position.y + offset);
        }

        Vector2 limitSize = new Vector2(0.5f * maxSize.x - size.x, 0.5f * maxSize.y - size.y);

        limitSize.x = Mathf.Clamp(transform.position.x, -limitSize.x + center.x, limitSize.x + center.x);
        limitSize.y = Mathf.Clamp(transform.position.y, -limitSize.y + center.y, limitSize.y + center.y);

        transform.position = new Vector3(limitSize.x, limitSize.y, -10.0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(center.x, center.y), maxSize);
    }
}
