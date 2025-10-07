using System;
using UniRx;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private int _direction;

    public event Action<int> ButtonPressed;
    public event Action UpButton;

    public void OnPressetButton()
    {
        ButtonPressed?.Invoke(_direction);
        MessageBroker.Default.Publish(new M_MoveElement());
    }

    public void OnUpButton()
    {
        UpButton?.Invoke();
        MessageBroker.Default.Publish(new M_StopMove());
    }
}