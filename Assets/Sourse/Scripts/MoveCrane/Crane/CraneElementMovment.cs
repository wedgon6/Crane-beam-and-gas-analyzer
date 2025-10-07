using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class CraneElementMovment : IMovebel
{
    private const float SMOOTHSTEP_MULTIPLIER = 3f;
    private const float SMOOTHSTEP_SUBTRACTOR = 2f;

    private float _craneForwardLimit;
    private float _craneBackwardLimit;
    private float _currentPosition = 0;
    private float _speed;
    private Transform _crane;
    private AxisType _movementAxis;
    private CancellationTokenSource _movementCTS;

    public CraneElementMovment(float speed, Transform carne,
        float forwardLimit,
        float backLomot, AxisType axis)
    {
        _speed = speed;
        _crane = carne;
        _craneForwardLimit = forwardLimit;
        _craneBackwardLimit = backLomot;
        _movementAxis = axis;
        UpdatePosition();
    }

    public async UniTaskVoid Move(int direction)
    {
        _movementCTS = new CancellationTokenSource();

        await ApplyMovement(direction, _movementCTS.Token);
    }

    public void StopMove()
    {
        _movementCTS?.Cancel();
        _movementCTS?.Dispose();
        _movementCTS = null;
    }

    private async UniTask ApplyMovement(int direction, CancellationToken ct)
    {
        if (ct.IsCancellationRequested)
            return;

        float startPosition = _currentPosition;
        float targetPosition = direction > 0 ? _craneForwardLimit : _craneBackwardLimit;

        if (Mathf.Approximately(_currentPosition, targetPosition))
            return;

        float distance = Mathf.Abs(targetPosition - startPosition);
        float duration = distance / Mathf.Abs(_speed);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (ct.IsCancellationRequested)
                return;

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            float smoothedT = SmoothStep(t);
            _currentPosition = Mathf.Lerp(startPosition, targetPosition, smoothedT);

            UpdatePosition();
            await UniTask.Yield();
        }

        if (!ct.IsCancellationRequested)
        {
            _currentPosition = targetPosition;
            UpdatePosition();
        }
    }
    private float SmoothStep(float t)
    {
        return t * t * (SMOOTHSTEP_MULTIPLIER - SMOOTHSTEP_SUBTRACTOR * t);
    }
    private void UpdatePosition()
    {
        Vector3 newPosition = _crane.localPosition;

        switch (_movementAxis)
        {
            case AxisType.X:
                newPosition.x = _currentPosition;
                break;
            case AxisType.Z:
                newPosition.z = _currentPosition;
                break;
            case AxisType.Y:
                newPosition.y = _currentPosition;
                break;
        }

        _crane.localPosition = newPosition;
    }
}