using DG.Tweening;
using UniRx;
using UnityEngine;

public class CoilRotate : MonoBehaviour
{
    private readonly float _durationRotation = 1f;
    private readonly Vector3 _rotationVector = new Vector3(360f, 0, 0);

    private Transform _transform;
    private CompositeDisposable _disposables = new();
    private Tweener _rotationTween;

    private void Start()
    {
        _transform = GetComponent<Transform>();

        MessageBroker.Default
                   .Receive<M_MoveElement>()
                   .Subscribe(m => PlayAudio())
                   .AddTo(_disposables);

        MessageBroker.Default
              .Receive<M_StopMove>()
              .Subscribe(m => StopAudio())
              .AddTo(_disposables);
    }

    private void OnDestroy()
    {
        if (_disposables != null)
            _disposables.Dispose();
    }

    private void PlayAudio()
    {
        _rotationTween = _transform.DORotate(_rotationVector, _durationRotation,
                RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetRelative().SetEase(Ease.Linear);
    }

    private void StopAudio()
    {
        _rotationTween?.Kill();
    }
}