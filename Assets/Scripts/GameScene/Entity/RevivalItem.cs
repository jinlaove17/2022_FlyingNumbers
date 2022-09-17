using UnityEngine;

public class RevivalItem : Entity
{
    private void OnEnable()
    {
        rigidBody2D.velocity = Vector2.zero;
    }

    private void OnDisable()
    {
        PoolingManager.Return(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.CanRebirth = true;
            SoundManager.Instance.PlaySFX("Buff Item");

            gameObject.SetActive(false);
        }
    }
}
