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

            // �浹 �Ŀ��� ���̻� �Ʒ��� �������� �ʵ��� rigidyBody�� simulated�� false�� �ٲ۴�.
            rigidBody2D.simulated = false;

            // ��ֹ��� ���, �ٸ� �����۰� �޸� �÷��̾�� �浹 �� ��ٷ� ������� ���� �ƴ϶� ��� �ִϸ��̼��� ����� �� ������� �Ǳ� ������,
            // �� ���� �浹�� �Ͼ���� BoxCollider2D ������Ʈ�� ��Ȱ��ȭ��Ų��.
            boxCollider2D.enabled = false;

            // �÷����� ü���� 0�̵��� �ʵ��� �ڷ�ƾ�� �ߴ��Ѵ�.
            //UIManager.Instance.InGameUIs.StatusUI.StopDecreasingHp();

            // �÷��̾ IDLE�̰ų� JUMP ���� ���� ��ֹ��� �µ��� �����Ѵ�.
            // �׷��� ������, �߸��� ��ȣ�� ���� ����ǥ�� ���� �� ��ֹ��� �� �浹�Ѵ�.
            if (GameManager.Instance.Player.State == PLAYER_STATE.IDLE || GameManager.Instance.Player.State == PLAYER_STATE.JUMPING)
            {
                GameManager.Instance.Player.State = PLAYER_STATE.HITTING;
            }

            SoundManager.Instance.PlaySFX("Impact");
        }
    }
}
