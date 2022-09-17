using UnityEngine;

public class Siren : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Alarm(float x)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(new Vector3(x, 0.0f, 0.0f));

        gameObject.SetActive(true);
        transform.position = new Vector2(screenPosition.x, transform.position.y);
        animator.Play("Alarm", -1, 0.0f);

        SoundManager.Instance.PlaySFX("Siren");
    }

    public void Inactive()
    {
        gameObject.SetActive(false);
    }
}
