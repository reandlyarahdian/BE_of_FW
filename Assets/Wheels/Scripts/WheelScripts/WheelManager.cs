using System;
using System.Collections;
using System.Collections.Generic;
using Watermelon;
using UnityEngine;
using UnityEngine.UI;

public class WheelManager : MonoBehaviour
{

    [Header("Coins Label")]
    [SerializeField] UIScaleAnimation coinsPanelScalable;
    [SerializeField] CurrencyUIPanelSimple coinsPanelUI;
    [Space]
    [SerializeField] private RewardFactory rewardFactory;
    [SerializeField] private SpinBehaviour spinBehaviour;
    [SerializeField] private Transform collectDestination;
    [SerializeField] private float collectDuration = 2.0f;
    [SerializeField] private float delay = 2.0f;
    [SerializeField] private bool unscaledTime;
    private List<IReward> rewardList;
    private IReward collectableReward;

    public event Action CollectEnded;

    [Header("References :")]
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private Transform linesParent;

    [Space]
    [SerializeField] private Transform PickerWheelTransform;
    [SerializeField] private Transform wheelCircle;
    [SerializeField] private GameObject wheelPiecePrefab;
    [SerializeField] private Transform wheelPiecesParent;

    [Space]
    [Header("Wheel :")]
    public List<RewardBehaviour> wheelPieces;

    private Vector2 pieceMinSize = new Vector2(81f, 146f);
    private Vector2 pieceMaxSize = new Vector2(144f, 213f);
    private int piecesMin = 2;
    private int piecesMax = 12;

    private float pieceAngle;
    private float halfPieceAngle;
    private float halfPieceAngleWithPaddings;

    private int currentReward;

    private int coinsHash = FloatingCloud.StringToHash("Coins");

    private void Start()
    {
        pieceAngle = 360 / RewardManager.Instance.rewardObjects.Length;
        halfPieceAngle = pieceAngle / 2f;
        halfPieceAngleWithPaddings = halfPieceAngle - (halfPieceAngle / 4f);

        Generate();
    }

    private void Generate()
    {
        rewardList = new List<IReward>();
        wheelPiecePrefab = InstantiatePiece();
        RectTransform rt = wheelPiecePrefab.transform.GetChild(0).GetComponent<RectTransform>();
        float pieceWidth = Mathf.Lerp(pieceMinSize.x, pieceMaxSize.x, 1f - Mathf.InverseLerp(piecesMin, piecesMax, RewardManager.Instance.rewardObjects.Length));
        float pieceHeight = Mathf.Lerp(pieceMinSize.y, pieceMaxSize.y, 1f - Mathf.InverseLerp(piecesMin, piecesMax, RewardManager.Instance.rewardObjects.Length));
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pieceWidth);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, pieceHeight);

        for (int i = 0; i < RewardManager.Instance.rewardObjects.Length; i++)
            DrawPiece(i);

        Destroy(wheelPiecePrefab);
    }

    private void DrawPiece(int index)
    {
        RewardBehaviour pices = new RewardBehaviour();
        GameObject p = InstantiatePiece();
        IReward reward;
        //Line
        Transform lineTrns = Instantiate(linePrefab, linesParent.position, Quaternion.identity, linesParent).transform;
        lineTrns.RotateAround(wheelPiecesParent.position, Vector3.back, (pieceAngle * index) + halfPieceAngle);

        p.transform.RotateAround(wheelPiecesParent.position, Vector3.back, pieceAngle * index);
        pices = p.GetComponent<RewardBehaviour>();

        reward = rewardFactory.GetReward(index);

        pices.Initialize(reward);
        rewardList.Add(reward);
        wheelPieces.Add(pices);
    }

    private GameObject InstantiatePiece()
    {
        return Instantiate(wheelPiecePrefab, wheelPiecesParent.position, Quaternion.identity, wheelPiecesParent);
    }

    public void SetReward()
    {
        int rndmRange = UnityEngine.Random.Range(0, rewardList.Count);
        collectableReward = rewardList[rndmRange];

        spinBehaviour.StartSpin(collectableReward.id, rewardList.Count);
    }

    public void CollectReward()
    {
        Image rewardImage = GetRewardBehaviour().iconRenderer;
        Vector2 maxScale = new Vector2(2.5f, 2.5f);
        
        FloatingCloud.SpawnCurrency(coinsHash, rewardImage.rectTransform, coinsPanelScalable.RectTransform, 10, "");

        CurrenciesController.Add(CollectableTypeEx(), CollectableRewordEx());

        ClientManager.Instance.WheelsPoints(CollectableRewordEx());

        rewardImage.transform.DOScale(maxScale, collectDuration / 2);
        rewardImage.transform.DOMove(collectDestination.position, collectDuration, delay, unscaledTime, UpdateMethod.LateUpdate).OnComplete(CollecEndHandler);
    }

    public int CollectableRewordEx()
    {
        return collectableReward.value;
    }

    public CurrencyType CollectableTypeEx()
    {
        return collectableReward.currencyType;
    }

    public void CollecEndHandler()
    {
        GetRewardBehaviour().ResetTransform();

        CollectEnded?.Invoke();
    }

    public void NextStart()
    {
        Debug.Log("Reward is " + collectableReward.name +
                    " | Value: " + collectableReward.value +
                        " | ID: " + collectableReward.id +
                        " | Type: " + collectableReward.currencyType.ToString());
    }

    public RewardBehaviour GetRewardBehaviour()
    {
        foreach (RewardBehaviour reward in wheelPieces)
        {
            if(reward.data.id == collectableReward.id)
                return reward;
        }

        return null;
    }
}
