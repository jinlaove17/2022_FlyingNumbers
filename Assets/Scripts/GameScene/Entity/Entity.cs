using UnityEngine;

public class Entity : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;

    protected Rigidbody2D rigidBody2D;
    protected BoxCollider2D boxCollider2D;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    protected virtual void Update()
    {
        CheckVisibility();
    }

    public virtual void Init()
    {

    }

    public virtual void Terminate()
    {

    }

    protected virtual void CheckVisibility()
    {
        // 카메라 영역의 최소 y값
        float minY = Camera.main.transform.position.y - Camera.main.orthographicSize;

        // 현재 오브젝트의 최대 y값
        float maxY = transform.position.y + 0.5f * spriteRenderer.bounds.size.y;

        if (minY >= maxY)
        {
            gameObject.SetActive(false);
        }
    }

    protected virtual void Inactive()
    {
        gameObject.SetActive(false);
    }
}
