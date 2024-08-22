using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Watermelon;

public class RewardBehaviour : MonoBehaviour
{
    [SerializeField] public Image iconRenderer;
    [SerializeField] private TextMeshProUGUI value;
    [SerializeField] private CurrencyType curr;
    private Vector2 imageStartPosition;
    private Vector2 imageStartScale;
    private IReward _data;

    public IReward data{ get => _data; }

    public void Initialize(IReward rewardData)
    {
        this._data = rewardData;
        imageStartPosition = iconRenderer.transform.localPosition;
        imageStartScale = iconRenderer.transform.localScale;

        SetIcon();
        SetName();
        SetValue();
    }

    private void SetIcon()
    {
        iconRenderer.sprite = _data.icon;
    }

    private void SetName()
    {
        transform.name = _data.name;
    }

    private void SetValue()
    {
        value.text = _data.value.ToString();
    }

    private void SetCurrency()
    {
        curr = _data.currencyType;
    }

    public void ResetTransform()
    {
        iconRenderer.transform.localPosition = imageStartPosition;
        iconRenderer.transform.localScale = imageStartScale;
    }
}
