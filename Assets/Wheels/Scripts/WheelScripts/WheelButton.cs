using System;
using UnityEngine;

public class WheelButton : MonoBehaviour
{
    public event Action ButtonNameChanged;

    public WheelManager wheelManager;

    private string _buttonText = "Spin";
    public string btnText{ get => _buttonText; set => _buttonText.ToString(); }

    public void SpinAvailable()
    {
        _buttonText = "Spin";

        ButtonNameHandler();
    }

    public void ClaimAvailable()
    {
        _buttonText = "Claim " + wheelManager.CollectableRewordEx();

        ButtonNameHandler();
    }

    public void NextAvailable()
    {
        _buttonText = "Back";

        ButtonNameHandler();
    }

    private void ButtonNameHandler()
    {
        ButtonNameChanged?.Invoke();
    }
}
