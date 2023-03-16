using System;
using System.Collections.Generic;
using DG.Tweening;
using Managers.Inventory;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FortuneWheel : MonoBehaviour
{
    [Header("REFERENCES:")]
    [SerializeField] private Image wheelCircle;
    [SerializeField] private Image indicator;
    [SerializeField] private Transform wheelPiecesParent;
    [SerializeField] private Button spinButton;
    [SerializeField] private Wheel[] wheels;

    private Wheel currentWheel;
    private UIManager uiManager;

    [Space] [Header("WHEEL SETTINGS:")] 
    [Range(1, 20)] public int spinDuration = 8;
    [SerializeField] [Range(.2f, 2f)] private float wheelSize = 1f;
    
    [HideInInspector] public InventorySlot[] slices = new InventorySlot[8];

    // Events
    public static Action onSpinStartEvent;
    public static Action<InventorySlot> onSpinEndEvent;
    
    private bool isSpinning, canSpin;
    private const float PieceAngle = 45;
    private double accumulatedWeight;
    private System.Random rand = new System.Random();
    private List<int> nonZeroChancesIndices = new List<int>();

    private void Start()
    {
        uiManager = UIManager.Instance;

        SetWheel();
    }

    private static int SetRandomAmountOfPrize(int ratio, int maxValue)
    {
        var randomAmount = Random.Range(1, maxValue / ratio);
        return randomAmount * ratio;
    }

    private void DrawSlice(int index)
    {
        var piece = slices[index];
        var pieceTransform = wheelPiecesParent.GetChild(index);

        if (pieceTransform.GetChild(0).TryGetComponent(out Image image)) image.sprite = piece.prize.icon;
        image.enabled = true;
        if (pieceTransform.GetChild(1).TryGetComponent(out TMP_Text text)) text.text = Inventory.SetAmountText(piece.amount);
    }

    private void GenerateSlices()
    {
        for (var i = 0; i < slices.Length; i++)
            DrawSlice(i);
    }

    private void SelectRandomPrizes()
    {
        var bombIndex = -1;
        if (!uiManager.IsSuperSafeZone() && !uiManager.IsSafeZone())
        {
            bombIndex = Random.Range(0, slices.Length);
            slices[bombIndex].prize = uiManager.bomb;
            slices[bombIndex].amount = 0;
        }

        for (var i = 0; i < slices.Length; i++)
        {
            if (i == bombIndex) continue;
            var randomItem = Random.Range(0, currentWheel.prizes.Length);
            slices[i].prize = currentWheel.prizes[randomItem];
            slices[i].amount = SetRandomAmountOfPrize(slices[i].prize.ratio, slices[i].prize.maxValue);
        }
    }

    private void SetCurrentWheelType()
    {
        if (uiManager.IsSuperSafeZone()) currentWheel = wheels[2];
        else if (uiManager.IsSafeZone()) currentWheel = wheels[1];
        else currentWheel = wheels[0];
    }

    public void SetWheel()
    {
        SetCurrentWheelType();

        wheelCircle.sprite = currentWheel.wheelSprite;
        indicator.sprite = currentWheel.indicatorSprite;

        SelectRandomPrizes();
        GenerateSlices();
        CalculateWeightsAndIndices();
        canSpin = true;
    }


    public void Spin()
    {
        if (isSpinning || !canSpin) return;
        
        isSpinning = true;
        canSpin = false;
        onSpinStartEvent?.Invoke();

        var index = GetRandomPieceIndex();
        var piece = slices[index];

        if (piece.prize.chance == 0 && nonZeroChancesIndices.Count != 0)
        {
            index = nonZeroChancesIndices[Random.Range(0, nonZeroChancesIndices.Count)];
            piece = slices[index];
        }

        var angle = -(PieceAngle * index);
        var targetRotation = Vector3.back * (angle + 2 * 360 * spinDuration);

        //float prevAngle = wheelCircle.eulerAngles.z + halfPieceAngle ;
        float prevAngle, currentAngle;
        prevAngle = currentAngle = wheelCircle.transform.eulerAngles.z;

        var isIndicatorOnTheLine = false;

        wheelCircle.transform
            .DORotate(targetRotation, spinDuration, RotateMode.Fast)
            .SetEase(Ease.InOutQuart)
            .OnUpdate(() =>
            {
                var diff = Mathf.Abs(prevAngle - currentAngle);
                if (diff >= PieceAngle)
                {
                    prevAngle = currentAngle;
                    isIndicatorOnTheLine = !isIndicatorOnTheLine;
                }

                currentAngle = wheelCircle.transform.eulerAngles.z;
            })
            .OnComplete(() =>
            {
                isSpinning = false;
                onSpinEndEvent?.Invoke(piece);
            });
    }

    private int GetRandomPieceIndex()
    {
        var randomPiece = rand.NextDouble() * accumulatedWeight;

        for (var i = 0; i < slices.Length; i++)
            if (slices[i].prize.weight >= randomPiece)
                return i;

        return 0;
    }

    private void CalculateWeightsAndIndices()
    {
        for (var i = 0; i < slices.Length; i++)
        {
            var piece = slices[i];

            //add weights:
            accumulatedWeight += piece.prize.chance;
            piece.prize.weight = accumulatedWeight;

            //add index :
            piece.prize.index = i;

            //save non zero chance indices:
            if (piece.prize.chance > 0)
                nonZeroChancesIndices.Add(i);
        }
    }


    private void OnValidate()
    {
        transform.localScale = new Vector3(wheelSize, wheelSize, 1f);
        spinButton.onClick.AddListener(Spin);
    }
}