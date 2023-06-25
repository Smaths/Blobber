using UnityEngine;

public class AimCanvasAtCamera : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Canvas _canvas;

    private void OnValidate()
    {
        _camera ??= Camera.main;
        _canvas ??= GetComponent<Canvas>();
    }

    private void LateUpdate()
    {
        if (_camera)
        {
            // Rotate the canvas to face the camera
            _canvas.transform.rotation = _camera.transform.rotation;
        }
    }
}
