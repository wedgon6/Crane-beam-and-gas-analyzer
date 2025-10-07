using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UniRx;
using UnityEngine;

public class Display : MonoBehaviour
{
    private readonly Color _enebleColor = Color.white;
    private readonly Color _disableColor = Color.black;
    private readonly Color _indicatorDisableColor = Color.red;
    private readonly Color _indicatorEnebleColor = Color.green;

    [SerializeField] private TMP_Text _text;
    [SerializeField] private Material _displayMaterial;
    [SerializeField] private Material _indicator;

    private Color _targetColor;
    private Color _currentColor;
    private CancellationTokenSource _fadeCTS;
    private float _fadeDuration;
    private float _fadeTimer;
    private bool _isEneble;
    private CompositeDisposable _disposables = new();

    private void OnDestroy()
    {
        if (_disposables != null)
            _disposables.Dispose();
    }

    public void Initialize(float fadeDuration, bool isAcrive)
    {
        _fadeDuration = fadeDuration;
        _isEneble = isAcrive;

        if (_isEneble)
        {
            _currentColor = _enebleColor;
            _displayMaterial.color = _currentColor;
            _indicator.color = _indicatorEnebleColor;
        }
        else
        {
            _currentColor = _disableColor;
            _displayMaterial.color = _currentColor;
            _indicator.color = _indicatorDisableColor;
        }

        _text.gameObject.SetActive(_isEneble);

        MessageBroker.Default
              .Receive<M_ButtonPressed>()
              .Subscribe(m => OnButtonPressed(m.Status))
              .AddTo(_disposables);

        MessageBroker.Default
             .Receive<M_ButtonRaised>()
             .Subscribe(m => OnButtonReseted())
             .AddTo(_disposables);

        MessageBroker.Default
             .Receive<M_DistanceChenged>()
             .Subscribe(m => OnDistanceChenged(m.Distance))
             .AddTo(_disposables);
    }

    private void OnDistanceChenged(float distance)
    {
        _text.text = distance.ToString();
    }

    private void OnButtonPressed(bool currentStatus)
    {
        if (currentStatus)
            StartFade(_enebleColor, _indicatorEnebleColor);
        else
            StartFade(_disableColor, _indicatorDisableColor);

        _isEneble = currentStatus;
    }

    private void OnButtonReseted()
    {
        StopFade();
    }

    private void StopFade()
    {
        _fadeCTS?.Cancel();
        _fadeCTS?.Dispose();
        _fadeCTS = null;
    }

    private async void StartFade(Color targetColor, Color indicatorColor)
    {
        StopFade();

        _fadeCTS = new CancellationTokenSource();

        await FadeToColor(targetColor, _fadeCTS.Token, indicatorColor);
    }

    private async UniTask FadeToColor(Color targetColor, CancellationToken ct, Color indicatorColor)
    {
        _displayMaterial.color = _currentColor;
        _targetColor = targetColor;
        _fadeTimer = 0f;

        try
        {
            while (_fadeTimer < _fadeDuration)
            {
                ct.ThrowIfCancellationRequested();

                _fadeTimer += Time.deltaTime;
                float progress = Mathf.Clamp01(_fadeTimer / _fadeDuration);

                Color currentColor = Color.Lerp(_currentColor, _targetColor, progress);
                _displayMaterial.color = currentColor;


                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            _displayMaterial.color = _targetColor;
            _indicator.color = indicatorColor;
            _currentColor = targetColor;
            _text.gameObject.SetActive(_isEneble);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Fade animation cancelled");
        }
        finally
        {
            StopFade();
        }
    }
}