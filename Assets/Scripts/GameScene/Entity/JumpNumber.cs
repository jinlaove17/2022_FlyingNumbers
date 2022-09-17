using UnityEngine;

public class JumpNumber : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private int number;

    public int Number
    {
        get
        {
            return number;
        }

        set
        {
            number = value;
            spriteRenderer.sprite = PlatformManager.Instance.NumberSprites[value - 1];
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
