using UnityEngine;

public class MoveBox : MonoBehaviour
{
    [SerializeField] private Vector2 positionInCell = Vector2.zero;
    [SerializeField] private float widthInCell = 5f;
    [SerializeField] private float heightInCell = 5f;
    [SerializeField] private float boundsWidth;
    [SerializeField] private FourDirectional<Transform> boundsTransforms;
    [SerializeField] private Grid grid;

    private void OnValidate()
    {
        if (!grid) return;
        if (!boundsTransforms.up || !boundsTransforms.down || !boundsTransforms.left || !boundsTransforms.right) return;

        float width = widthInCell * grid.transform.localScale.x;
        float height = heightInCell * grid.transform.localScale.y;
        Vector2 position = new Vector2(
            positionInCell.x * grid.transform.localScale.x,
            positionInCell.y * grid.transform.localScale.y);
        
        boundsTransforms.up.localScale = new Vector3(width + boundsWidth * 2f, boundsWidth, 1f);
        boundsTransforms.down.localScale = new Vector3(width + boundsWidth * 2f, boundsWidth, 1f);
        boundsTransforms.left.localScale = new Vector3(boundsWidth, height + boundsWidth * 2f, 1f);
        boundsTransforms.right.localScale = new Vector3(boundsWidth, height + boundsWidth * 2f, 1f);
        boundsTransforms.up.transform.localPosition = position + new Vector2(0f, height / 2f + boundsWidth / 2f);
        boundsTransforms.down.transform.localPosition = position + new Vector2(0f, -height / 2f - boundsWidth / 2f);
        boundsTransforms.left.transform.localPosition = position + new Vector2(width / 2f + boundsWidth / 2f, 0f);
        boundsTransforms.right.transform.localPosition = position + new Vector2(-width / 2f - boundsWidth / 2f, 0f);
    }
}
