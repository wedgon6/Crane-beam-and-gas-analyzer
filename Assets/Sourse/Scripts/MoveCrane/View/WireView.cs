using UnityEngine;

public class WireView : MonoBehaviour
{
    [SerializeField] private Transform _topPoint;
    [SerializeField] private Transform _hook;
    [SerializeField] private LineRenderer _lineRenderer;

    private Vector3 _lastPosition;

    private void Start()
    {
        _lastPosition = _hook.position;
        UpdateLine();
    }

    private void LateUpdate()
    {
        if (_lastPosition != _hook.position)
        {
            UpdateLine();
            _lastPosition = _hook.position;
        }
    }

    private void UpdateLine()
    {
        Vector3[] cablePoints = new Vector3[2];
        cablePoints[0] = _topPoint.position;
        cablePoints[1] = _hook.position;

        _lineRenderer.SetPositions(cablePoints);
    }
}