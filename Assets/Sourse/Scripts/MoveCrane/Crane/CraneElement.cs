using UnityEngine;

public class CraneElement : MonoBehaviour
{
    [SerializeField] private float _craneForwardLimit = 20f;
    [SerializeField] private float _craneBackwardLimit = -20f;
    [SerializeField] private float _speed;
    [SerializeField] private ButtonController _controllerButtonForwardon;
    [SerializeField] private ButtonController _controllerButtonBack;
    [SerializeField] private AxisType _axisType;

    private CraneElementMovment _movment;

    private void Awake()
    {
        _movment = new(_speed, gameObject.transform, _craneForwardLimit, _craneBackwardLimit, _axisType);
    }

    private void OnEnable()
    {
        _controllerButtonForwardon.ButtonPressed += Move;
        _controllerButtonForwardon.UpButton += StopMove;
        _controllerButtonBack.ButtonPressed += Move;
        _controllerButtonBack.UpButton += StopMove;
    }

    private void StopMove()
    {
        _movment.StopMove();
    }

    private void Move(int direction)
    {
        _ = _movment.Move(direction);
    }
}