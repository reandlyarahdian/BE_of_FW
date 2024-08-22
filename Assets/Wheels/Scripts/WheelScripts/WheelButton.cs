using System;
using UnityEngine;

public class WheelButton : MonoBehaviour
{
    public event Action ButtonNameChanged;

    private string _buttonText = "Spin";
    public string btnText{ get => _buttonText; set => _buttonText.ToString(); }

    public void SpinAvailable()
    {
        _buttonText = "Spin";

        ButtonNameHandler();
    }

    public void ClaimAvailable()
    {
        _buttonText = "Claim";

        ButtonNameHandler();
    }

    public void NextAvailable()
    {
        _buttonText = "Next";

        ButtonNameHandler();
    }

    private void ButtonNameHandler()
    {
        ButtonNameChanged?.Invoke();
    }
}
