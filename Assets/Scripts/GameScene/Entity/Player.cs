using System.Collections;
using UnityEngine;

public enum PLAYER_STATE
{
    IDLE,
    JUMPING,
    HITTING,
    FALLING,
    REVIVING
}

public class Player : Entity
{
    private int platformLayerMask;

    private PLAYER_STATE state;

    [SerializeField]
    private float jumpPower;

    public PLAYER_STATE State
    {
        get
        {
            return state;
        }

        set
        {
            state = value;

            switch (state)
            {
                case PLAYER_STATE.HITTING:
                    state = PLAYER_STATE.FALLING;
                    StartCoroutine(Fall(true));
                    break;
                case PLAYER_STATE.FALLING:
                    StartCoroutine(Fall(false));
                    break;
                case PLAYER_STATE.REVIVING:
                    StartCoroutine(Revive());
                    break;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();

        platformLayerMask = LayerMask.GetMask("Platform");
    }

    protected override void Update()
    {
        if (GameManager.Instance.IsGameStart)
        {
            switch (state)
            {
                case PLAYER_STATE.JUMPING:
                    SetAnimation();
                    break;
                case PLAYER_STATE.FALLING:
                    CheckVisibility();
                    break;
            }
        }
    }

    public override void Init()
    {
        state = PLAYER_STATE.JUMPING;
        animator.SetBool("Jump", true);
        rigidBody2D.AddForce(0.8f * jumpPower * Vector2.up, ForceMode2D.Impulse);

        SoundManager.Instance.PlaySFX("Jump");
    }

    public override void Terminate()
    {
        state = PLAYER_STATE.IDLE;
        animator.SetBool("Fall", false);
        spriteRenderer.flipX = false;
        rigidBody2D.position = new Vector3(0.0f, -1.0f, 0.0f);
        rigidBody2D.velocity = Vector2.zero;
        boxCollider2D.enabled = true;
    }

    protected override void CheckVisibility()
    {
        // ī�޶� ������ �ּ� y��
        float minY = Camera.main.transform.position.y - Camera.main.orthographicSize;

        // ���� ������Ʈ�� �ִ� y��
        float maxY = rigidBody2D.position.y + 0.5f * spriteRenderer.bounds.size.y;

        if (minY >= maxY)
        {
            if (GameManager.Instance.CanRebirth)
            {
                State = PLAYER_STATE.REVIVING;
            }
            else
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    private void SetDirection()
    {
        if (rigidBody2D.velocity.x < 0.0f)
        {
            spriteRenderer.flipX = true;
        }
        else if (rigidBody2D.velocity.x > 0.0f)
        {
            spriteRenderer.flipX = false;
        }
    }

    private void SetAnimation()
    {
        if (rigidBody2D.velocity.y < 0.0f)
        {
            // ������ �÷����� ���� ��쿡,
            if (PlatformManager.Instance.CurrentPlatform == null)
            {
                // ���� ĳ������ �̿��Ͽ� ���� ���θ� �˻��Ѵ�.
                Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y - 0.5f * spriteRenderer.bounds.size.y);
                RaycastHit2D raycastHit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.1f, platformLayerMask);

                if (raycastHit.collider != null)
                {
                    if (raycastHit.collider.gameObject.TryGetComponent(out Platform platform))
                    {
                        // isTrigger�� ���� ��쿡��, ���� ���� ���¸� �ǹ��Ѵ�.
                        if (raycastHit.collider.isTrigger)
                        {
                            raycastHit.collider.isTrigger = false;

                            state = PLAYER_STATE.IDLE;
                            animator.SetBool("Jump", false);
                            rigidBody2D.MovePosition(new Vector2(platform.transform.position.x, transform.position.y));
                            rigidBody2D.velocity = Vector2.zero;

                            PlatformManager.Instance.CurrentPlatform = platform;
                        }
                    }
                }
            }
        }
    }

    public void Jump(Vector2 direction)
    {
        if (PlatformManager.Instance.CurrentPlatform != null)
        {
            state = PLAYER_STATE.JUMPING;
            animator.SetBool("Jump", true);

            // ����� ������ y�� �ʹ� Ŀ�� ������ �ʰ�, �¿� ������ x�� �ʹ� Ŀ�� �ָ� ���ư��� ���� �����ϱ� ���� ���Ⱚ�� �����Ͽ� �������� ���δ�.
            if (Mathf.Abs(direction.x) <= 0.01f)
            {
                direction.y *= 0.5f;
            }
            else
            {
                direction.x *= 0.7f;
            }

            rigidBody2D.AddForce(jumpPower * direction, ForceMode2D.Impulse);
            
            SetDirection();

            PlatformManager.Instance.CurrentPlatform.Bounce();
            PlatformManager.Instance.CurrentPlatform = null;
            UIManager.Instance.InGameUIs.ScoreUI.IncreaseScore();
            UIManager.Instance.InGameUIs.StatusUI.IncreaseHp();
            SoundManager.Instance.PlaySFX("Jump");
        }
    }

    private IEnumerator Fall(bool isHit)
    {
        // ��ֹ��� �¾Ƽ� �������� ���� �ƴ϶��, ����ǥ�� ����ϰ� �����ð� �Ŀ� ����߸���.
        if (!isHit)
        {
            // ����ǥ�� ����Ѵ�.
            UIManager.Instance.InGameUIs.GameOverUI.ShowExclamationMark(new Vector2(transform.position.x, transform.position.y + 1.0f));

            yield return new WaitForSeconds(0.6f);
        }

        animator.SetBool("Jump", false);
        animator.SetBool("Fall", true);
        rigidBody2D.velocity = Vector2.zero;
        rigidBody2D.AddForce(jumpPower * Vector2.up, ForceMode2D.Impulse);

        // �����۰� �浹�� ���� ���� ��Ȱ��ȭ��Ų��.
        boxCollider2D.enabled = false;

        if (PlatformManager.Instance.CurrentPlatform != null)
        {
            PlatformManager.Instance.CurrentPlatform.Die();
        }
    }

    private IEnumerator Revive()
    {
        GameManager.Instance.CanRebirth = false;

        animator.SetBool("Fall", false);
        animator.SetTrigger("Revive");
        rigidBody2D.velocity = Vector2.zero;
        rigidBody2D.gravityScale = 0.0f;

        // LastPlatform�� �÷��̾ ���� ���¿��� �߶����� �ÿ�, CurrentPlatform�� null�� ������ �ذ��ϱ� ���� �߰��Ͽ���.
        Platform targetPlatform = PlatformManager.Instance.LastPlatform.NextPlatforms[0];
        Vector2 targetPosition = new Vector2(targetPlatform.transform.position.x, targetPlatform.transform.position.y + 0.9f * spriteRenderer.bounds.size.y);
        const float offset = 0.05f;

        while (true)
        {
            Vector2 newPosition = Vector2.Lerp(rigidBody2D.position, targetPosition, Time.deltaTime);

            rigidBody2D.MovePosition(newPosition);

            // offset��ŭ ������ �ְ�, �� ���������� ������ �ڷ�ƾ�� �����Ѵ�.
            if ((targetPosition.x - offset <= rigidBody2D.position.x && rigidBody2D.position.x <= targetPosition.x + offset) &&
                (targetPosition.y - offset <= rigidBody2D.position.y && rigidBody2D.position.y <= targetPosition.y + offset))
            {
                animator.SetTrigger("Revive To Fall");
                break;
            }

            yield return null;
        }

        rigidBody2D.position = new Vector2(targetPosition.x, targetPosition.y);
        rigidBody2D.gravityScale = 1.0f;
        boxCollider2D.enabled = true;
        state = PLAYER_STATE.JUMPING;

        UIManager.Instance.InGameUIs.StatusUI.IncreaseHpToFull();
        PlatformManager.Instance.CurrentPlatform = null;
    }
}
