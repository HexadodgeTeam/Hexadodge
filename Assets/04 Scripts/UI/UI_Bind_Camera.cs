using UnityEngine;

public class UI_Bind_Camera : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }
}
