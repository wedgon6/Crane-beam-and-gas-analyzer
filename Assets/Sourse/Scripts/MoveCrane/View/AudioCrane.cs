using UniRx;
using UnityEngine;

public class AudioCrane : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _moveSound;

    private CompositeDisposable _disposables = new();

    private void Start()
    {
        _audioSource.clip = _moveSound;
        _audioSource.playOnAwake = false;

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
        _audioSource.Play();
    }

    private void StopAudio()
    {
        _audioSource.Pause();
    }
}