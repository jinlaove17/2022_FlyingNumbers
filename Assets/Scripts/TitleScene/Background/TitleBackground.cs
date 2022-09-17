using UnityEngine;

public class TitleBackground : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    [SerializeField]
    private float speed;

    private float offset;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        meshRenderer.material.mainTextureOffset = new Vector3(-offset, offset);
        offset += speed * Time.deltaTime;
    }
}
