using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MinimapController : MonoBehaviour
{
    [SerializeField] private Camera _minimapCamera;

    // _minimapCamera.orthographicSize 값이 작을 수록 확대(줌인)
    [SerializeField] private float _zoomMin = 10f;
    
    [SerializeField] private float _zoomMax = 40f;

    [SerializeField] private float _zoomStep = 3f;

    [SerializeField] private TextMeshProUGUI _placeNameText;

    public void SetPlaceName(string placeName)
    {
        if (!_placeNameText) return;

        _placeNameText.text = placeName;
    }

    private void Start()
    {
        // 초기에 미니맵 줌을 최대 확대로
        if (_minimapCamera) _minimapCamera.orthographicSize = (int)(((_zoomMin + _zoomMax) / 2f) / _zoomStep) * _zoomStep;
    }

    public void ZoomIn()
    {
        if (!_minimapCamera) return;

        _minimapCamera.orthographicSize = Mathf.Max(_minimapCamera.orthographicSize - _zoomStep, _zoomMin);
    }

    public void ZoomOut()
    {
        if (!_minimapCamera) return;

        _minimapCamera.orthographicSize = Mathf.Min(_minimapCamera.orthographicSize + _zoomStep, _zoomMax);
    }

    public void LinkCamera(Camera camera)
    {
        _minimapCamera = camera;
    }
}
