using UnityEngine;
using Watermelon;

public class RewardManager : MonoBehaviour
{
    [SerializeField] private ScriptableRewardObject[] scriptableRewardObjectArray;
    [SerializeField] private ScriptableRewardObject dummyScriptableRewardObject;

    public ScriptableRewardObject[] rewardObjects {  get { return scriptableRewardObjectArray; } }

    static RewardManager instance;

    public static RewardManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("ScriptHolder").GetComponent<RewardManager>();

            return instance;
        }
    }

    public Sprite GetIcon(int id)
    {
        return rewardObject(id).icon;
    }

    public string GetName(int id)
    {
        return rewardObject(id).rewardName;
    }

    public int GetValue(int id)
    {
        return rewardObject(id).value;
    }

    public CurrencyType GetType(int id) 
    { 
        return rewardObject(id).currencyType;
    }

    private ScriptableRewardObject rewardObject(int id)
    {
        foreach (ScriptableRewardObject reward in scriptableRewardObjectArray)
        {
            if (reward.id == id)
                return reward;
        }

        return dummyScriptableRewardObject;
    }
}
