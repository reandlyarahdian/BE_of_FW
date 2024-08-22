using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

[CreateAssetMenu(fileName = "Reward_Data", menuName = "Create Reward/New Reward", order = 1)]
public class ScriptableRewardObject : ScriptableObject
{
    [SerializeField] public int id = 0;
    [SerializeField] public string rewardName = "Reward";
    [SerializeField] public CurrencyType currencyType;
    [SerializeField] public Sprite icon;
    [SerializeField] public int value = 0;
}
