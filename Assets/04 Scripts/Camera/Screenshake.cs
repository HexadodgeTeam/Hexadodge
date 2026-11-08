using System.Collections;
using UnityEngine;

public class Screenshake : MonoBehaviour
{
    public AnimationCurve curve;
    public float duration = 1f;
    [SerializeField] public Camera camera;

    IEnumerator Shaking()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }
        transform.position = startPosition;
    }
    
    
}
