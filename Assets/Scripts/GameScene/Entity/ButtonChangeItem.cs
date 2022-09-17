using UnityEngine;

public class ButtonChangeItem : Entity
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
            UIManager.Instance.InGameUIs.JumpButtonUI.ShuffleButton();
            SoundManager.Instance.PlaySFX("Debuff Item");

            gameObject.SetActive(false);
        }
    }
}
