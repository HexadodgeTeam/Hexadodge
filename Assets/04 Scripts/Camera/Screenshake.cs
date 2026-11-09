using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Screenshake : MonoBehaviour
{
    public AnimationCurve curve;
    public float duration = 1f;
    [SerializeField] public float strengthMultiplierWeak = 1f;
    [SerializeField] public float strengthMultiplierMid = 1f;
    [SerializeField] public float strengthMultiplierStrong = 1f;

    private Camera _camera;
    
    private void Awake()
    {
        _camera = Camera.main;
    }

    public IEnumerator ShakingStrong()
    {
        Vector3 startPosition = _camera.transform.position;
        
        float elapsedTime = 0;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            float strength = curve.Evaluate(elapsedTime / duration);
            
            _camera.transform.position = startPosition + Random.insideUnitSphere * strength * strengthMultiplierStrong;
            
            yield return null;
        }
        
        _camera.transform.position = startPosition;
    }
    
    public IEnumerator ShakingMid()
    {
        Vector3 startPosition = _camera.transform.position;
        
        float elapsedTime = 0;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            float strength = curve.Evaluate(elapsedTime / duration);
            
            _camera.transform.position = startPosition + Random.insideUnitSphere * strength * strengthMultiplierMid;
            
            yield return null;
        }
        
        _camera.transform.position = startPosition;
    }
    public IEnumerator ShakingWeak()
    {
        Vector3 startPosition = _camera.transform.position;
        
        float elapsedTime = 0;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            float strength = curve.Evaluate(elapsedTime / duration);
            
            _camera.transform.position = startPosition + Random.insideUnitSphere * strength * strengthMultiplierWeak;
            
            yield return null;
        }
        
        _camera.transform.position = startPosition;
    }
    
    
}
