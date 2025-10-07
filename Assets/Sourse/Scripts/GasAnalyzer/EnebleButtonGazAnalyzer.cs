using System;
using UnityEngine;

public class EnebleButtonGazAnalyzer : MonoBehaviour
{
    public event Action ButtonPressed;
    public event Action ButtonRaised;

    public void OnButtonPressed()
    {
        ButtonPressed?.Invoke();
    }

    public void OnButtonRaised()
    {
        ButtonRaised?.Invoke();
    }
}