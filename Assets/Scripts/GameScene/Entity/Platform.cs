using UnityEngine;

public class Platform : Entity
{
    private JumpNumber jumpNumber;

    [SerializeField]
    private Platform[] nextPlatforms;

    public int Number
    {
        get
        {
            return jumpNumber.Number;
        }

        set
        {
            jumpNumber.Number = value;
        }
    }

    public Platform[] NextPlatforms
    {
        get
        {
            return nextPlatforms;
        }

        set
        {
            nextPlatforms = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        jumpNumber = GetComponentInChildren<JumpNumber>();
        nextPlatforms = new Platform[2];
    }

    public void OnEnable()
    {
        boxCollider2D.isTrigger = true;
        jumpNumber.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        PoolingManager.Return(gameObject);
    }

    protected override void CheckVisibility()
    {
        // 카메라 영역의 최소 y값
        float minY = Camera.main.transform.position.y - Camera.main.orthographicSize;

        // 현재 오브젝트의 최대 y값
        float maxY = rigidBody2D.position.y + 0.5f * spriteRenderer.bounds.size.y;

        if (minY >= maxY)
        {
            gameObject.SetActive(false);

            --PlatformManager.Instance.PlatformCount;
        }
    }

    public void HideNumber()
    {
        jumpNumber.gameObject.SetActive(false);
    }

    public void Bounce()
    {
        animator.SetTrigger("Bounce");
    }

    public void Die()
    {
        animator.SetTrigger("Die");
    }

    protected override void Inactive()
    {
        base.Inactive();

        --PlatformManager.Instance.PlatformCount;
    }
}
