using UnityEngine;

public class Obstacle : Entity
{
    private void OnEnable()
    { 
        rigidBody2D.velocity = Vector2.zero;
        rigidBody2D.simulated = true;
        boxCollider2D.enabled = true;
    }

    private void OnDisable()
    {
        PoolingManager.Return(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetTrigger("Die");

            // 충돌 후에는 더이상 아래로 떨어지지 않도록 rigidyBody의 simulated를 false로 바꾼다.
            rigidBody2D.simulated = false;

            // 장애물의 경우, 다른 아이템과 달리 플레이어와 충돌 시 곧바로 사라지는 것이 아니라 사망 애니메이션을 출력한 후 사라져야 되기 때문에,
            // 한 번만 충돌이 일어나도록 BoxCollider2D 컴포넌트를 비활성화시킨다.
            boxCollider2D.enabled = false;

            // 플랫폼의 체력이 0이되지 않도록 코루틴을 중단한다.
            //UIManager.Instance.InGameUIs.StatusUI.StopDecreasingHp();

            // 플레이어가 IDLE이거나 JUMP 중일 때만 장애물에 맞도록 설정한다.
            // 그렇지 않으면, 잘못된 번호를 눌러 느낌표가 떴을 때 장애물과 또 충돌한다.
            if (GameManager.Instance.Player.State == PLAYER_STATE.IDLE || GameManager.Instance.Player.State == PLAYER_STATE.JUMPING)
            {
                GameManager.Instance.Player.State = PLAYER_STATE.HITTING;
            }

            SoundManager.Instance.PlaySFX("Impact");
        }
    }
}
