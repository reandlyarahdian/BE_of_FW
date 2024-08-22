using UnityEngine;
using Watermelon;

public interface IReward
{
    Sprite icon{ get; set; }
    string name{ get; set; }
    int value{ get; set; }
    CurrencyType currencyType{ get; set; }
    int id { get; set; }
}
