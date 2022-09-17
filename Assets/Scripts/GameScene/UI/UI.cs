using UnityEngine;

public class UI : MonoBehaviour
{
    protected Animator animator;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public virtual void Init()
    {

    }

    public virtual void Terminate()
    { 

    }
}
