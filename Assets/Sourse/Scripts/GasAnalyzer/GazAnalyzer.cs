using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UniRx;

public class GazAnalyzer : MonoBehaviour
{
    [SerializeField] private EnebleButtonGazAnalyzer _buttonController;
    [SerializeField] private float _timeSecondEnable;
    [SerializeField] private Display _display;
    [SerializeField] private Transform _zond;

    private DangerZoneFinder _dangerZoneFinder;
    private CancellationTokenSource _movementCTS;
    private bool _isEneble = false;

    private void OnEnable()
    {
        _buttonController.ButtonPressed += OnButtonPressed;
        _buttonController.ButtonRaised += StopCounter;
    }

    private void Start()
    {
        _display.Initialize(_timeSecondEnable, _isEneble);
        _dangerZoneFinder = new(_zond);
    }

    private void OnButtonPressed()
    {
        _ = StartCounter();
        MessageBroker.Default.Publish(new M_ButtonPressed(_isEneble));
        Debug.Log(_isEneble);
    }

    private async UniTaskVoid StartCounter()
    {
        _movementCTS = new CancellationTokenSource();

        try
        {
            await Counter(_movementCTS.Token);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Counter was cancelled");
        }
        finally
        {
            _movementCTS?.Dispose();
            _movementCTS = null;
            _isEneble = !_isEneble;
        }
    }

    private void StopCounter()
    {
        MessageBroker.Default.Publish(new M_ButtonRaised());

        if (_movementCTS == null)
            return;

        _movementCTS?.Cancel();
        _movementCTS?.Dispose();
        _movementCTS = null;
    }

    private async UniTask Counter(CancellationToken ct)
    {
        var startTime = Time.time;

        while (Time.time - startTime < _timeSecondEnable)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }

        _isEneble = !_isEneble;
        MessageBroker.Default.Publish(new M_EnebleStatusChanged(_isEneble));
    }
}