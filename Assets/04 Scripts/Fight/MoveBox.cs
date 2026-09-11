using UnityEngine;

public class MoveBox : MonoBehaviour
{
    [SerializeField] private Vector2 position = Vector2.zero;
    [SerializeField] private float width;
    [SerializeField] private float height;
    [SerializeField] private float boundsWidth;
    [SerializeField] private FourDirectional<Transform> boundsTransforms;

    private void OnValidate()
    {
        if (!boundsTransforms.up || !boundsTransforms.down || !boundsTransforms.left || !boundsTransforms.right) return;
        
        boundsTransforms.up.localScale = new Vector3(width + boundsWidth, boundsWidth, 1f);
        boundsTransforms.down.localScale = new Vector3(width + boundsWidth, boundsWidth, 1f);
        boundsTransforms.left.localScale = new Vector3(boundsWidth, height + boundsWidth, 1f);
        boundsTransforms.right.localScale = new Vector3(boundsWidth, height + boundsWidth, 1f);
        boundsTransforms.up.transform.position = position + new Vector2(0f, height / 2f);
        boundsTransforms.down.transform.position = position + new Vector2(0f, -height / 2f);
        boundsTransforms.left.transform.position = position + new Vector2(width / 2f, 0f);
        boundsTransforms.right.transform.position = position + new Vector2(-width / 2f, 0f);
    }
}
