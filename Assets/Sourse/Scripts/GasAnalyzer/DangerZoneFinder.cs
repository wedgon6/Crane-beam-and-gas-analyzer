using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;
using UnityEngine;

public class DangerZoneFinder
{
    private readonly float _searchRadius = 40f;
    private readonly int _searchDelay = 100;

    private float _lastDistance = 0;
    private float _nearestDistance;
    private Transform _zond;
    private Vector3 _lastZondPosition;
    private CompositeDisposable _disposables = new();
    private Collider[] _dangerZones = new Collider[50];
    private CancellationTokenSource _searchCTS;

    public DangerZoneFinder(Transform zond)
    {
        _zond = zond;

        MessageBroker.Default
             .Receive<M_EnebleStatusChanged>()
             .Subscribe(m => OnEnebleStatusChanged(m.IsEneble))
             .AddTo(_disposables);
    }

    private void OnEnebleStatusChanged(bool isEneble)
    {
        if (!isEneble)
            StartSearch();
        else
            StopSearch();
    }

    private async void StartSearch()
    {
        _searchCTS = new CancellationTokenSource();

        await FindDangerZone(_searchCTS.Token);
    }

    private void StopSearch()
    {
        _searchCTS?.Cancel();
        _searchCTS?.Dispose();
        _searchCTS = null;
    }

    private async UniTask FindDangerZone(CancellationToken ct)
    {
        _lastZondPosition = _zond.position.normalized;

        while (!ct.IsCancellationRequested)
        {
            if (_lastZondPosition != _zond.position)
            {
                if (_lastDistance != GetDangerZone())
                {
                    _lastDistance = _nearestDistance;
                    MessageBroker.Default.Publish(new M_DistanceChenged(_lastDistance));
                }

                _lastZondPosition = _zond.position.normalized;
            }

            await UniTask.Delay(_searchDelay);
        }
    }

    private float GetDangerZone()
    {
        _nearestDistance = float.MaxValue;

        int count = Physics.OverlapSphereNonAlloc(_zond.position,
            _searchRadius,
            _dangerZones
        );

        for (int i = 0; i < count; i++)
        {
            if (_dangerZones[i] != null &&
                _dangerZones[i].TryGetComponent(out DangerZone dangerZone))
            {
                float distance = Vector3.Distance(_zond.position, _dangerZones[i].transform.position);

                if (distance < _nearestDistance)
                    _nearestDistance = distance;
            }
        }

        return _nearestDistance;
    }
}