using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Watermelon;

public class WheelButtonPresenter : MonoBehaviour
{
    [SerializeField] private WheelButton wheelButton;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private WheelManager wheelManager;
    [SerializeField] private SpinBehaviour spinBehaviour;
    [SerializeField] private Button button;
    [SerializeField] private UIWheel wheel;

    private void OnEnable()
    {
        button.onClick.AddListener(SpinEventListener);

        wheelButton.ButtonNameChanged += ButtonNameChangeEvent;
        spinBehaviour.SpinEnded += SpinEndedHandler;
        wheelManager.CollectEnded += ClaimEndedHandler;
    }

    private void SpinEventListener()
    {
        button.interactable = false;

        wheelButton.SpinAvailable();
        wheelManager.SetReward();
    }
    private void ClaimEventListener()
    {
        button.interactable = false;
        
        wheelManager.CollectReward();
        wheel.isActiveated = true;
        button.onClick.AddListener(ClaimEndedHandler);
    }

    private void SpinEndedHandler()
    {
        button.interactable = true;

        wheelButton.ClaimAvailable();
        button.onClick.AddListener(ClaimEventListener);
    }

    private void ClaimEndedHandler()
    {
        button.interactable = true;

        wheelButton.NextAvailable();
        button.onClick.AddListener(NextLvl);
    }

    private void NextLvl()
    {
        button.interactable = false;

        wheel.WheelButton();
        wheelManager.NextStart();
    }

    private void UpdateButton()
    {
        buttonText.text = wheelButton.btnText;
        button.onClick.RemoveAllListeners();
    }

    private void ButtonNameChangeEvent()
    {
        UpdateButton();
    }
}