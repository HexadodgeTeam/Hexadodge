using System.Collections;
using UnityEngine;

public class Screenshake : MonoBehaviour
{
    public AnimationCurve curve;
    public float duration = 1f;
    [SerializeField] public new Camera camera;
    [SerializeField] public float strengthMultiplierWeak = 1f;
    [SerializeField] public float strengthMultiplierMid = 1f;
    [SerializeField] public float strengthMultiplierStrong = 1f;
    

    public IEnumerator ShakingStrong()
    {
        Vector3 startPosition = camera.transform.position;
        
        float elapsedTime = 0;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            float strength = curve.Evaluate(elapsedTime / duration);
            
            camera.transform.position = startPosition + Random.insideUnitSphere * strength * strengthMultiplierStrong;
            
            yield return null;
        }
        
        camera.transform.position = startPosition;
    }
    
    public IEnumerator ShakingMid()
    {
        Vector3 startPosition = camera.transform.position;
        
        float elapsedTime = 0;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            float strength = curve.Evaluate(elapsedTime / duration);
            
            camera.transform.position = startPosition + Random.insideUnitSphere * strength * strengthMultiplierMid;
            
            yield return null;
        }
        
        camera.transform.position = startPosition;
    }
    public IEnumerator ShakingWeak()
    {
        Vector3 startPosition = camera.transform.position;
        
        float elapsedTime = 0;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            float strength = curve.Evaluate(elapsedTime / duration);
            
            camera.transform.position = startPosition + Random.insideUnitSphere * strength * strengthMultiplierWeak;
            
            yield return null;
        }
        
        camera.transform.position = startPosition;
    }
    
    
}
