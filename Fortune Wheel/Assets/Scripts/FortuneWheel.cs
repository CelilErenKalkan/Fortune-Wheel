using System;
using System.Collections.Generic;
using DG.Tweening;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FortuneWheel : MonoBehaviour
{
    private Wheel currentWheel;
    private UIManager _uiManager;
    
    [Header("REFERENCES:")]
    [SerializeField] private Image wheelCircle;
    [SerializeField] private Image indicator;
    [SerializeField] private Transform wheelPiecesParent;
    [SerializeField] private Wheel[] wheels;

    [Space] [Header("SETTINGS:")] 
    [Range(1, 20)] public int spinDuration = 8;
    [SerializeField] [Range(.2f, 2f)] private float wheelSize = 1f;

    [Space] [Header("SLICES:")]
    [HideInInspector] public InventorySlot[] slices = new InventorySlot[8];

    // Events
    public static Action OnSpinStartEvent;
    public static Action<InventorySlot> OnSpinEndEvent;


    private bool isSpinning;
    private float pieceAngle;
    private double accumulatedWeight;
    private System.Random rand = new System.Random();
    private List<int> nonZeroChancesIndices = new List<int>();

    private void Start()
    {
        _uiManager = UIManager.Instance;
        pieceAngle = 360 / slices.Length;

        SetWheel();
    }

    private int SetRandomAmount(int ratio, int maxValue)
    {
        var randomAmount = Random.Range(1, maxValue / ratio);
        return randomAmount * ratio;
    }

    private void DrawPiece(int index)
    {
        var piece = slices[index];
        var pieceTransform = wheelPiecesParent.GetChild(index);

        if (pieceTransform.GetChild(0).TryGetComponent(out Image image)) image.sprite = piece.prize.icon;
        image.enabled = true;
        if (pieceTransform.GetChild(1).TryGetComponent(out TMP_Text text)) text.text = Inventory.SetAmountText(piece.amount);
    }

    private void Generate()
    {
        for (var i = 0; i < slices.Length; i++)
            DrawPiece(i);
    }

    private void SelectRandomPrizes()
    {
        var bombIndex = -1;
        if (!_uiManager.IsSuperSafeZone() && !_uiManager.IsSafeZone())
        {
            bombIndex = Random.Range(0, slices.Length);
            slices[bombIndex].prize = _uiManager.bomb;
            slices[bombIndex].amount = 0;
        }

        for (var i = 0; i < slices.Length; i++)
        {
            if (i == bombIndex) continue;
            var randomItem = Random.Range(0, currentWheel.prizes.Length);
            slices[i].prize = currentWheel.prizes[randomItem];
            slices[i].amount = SetRandomAmount(slices[i].prize.ratio, slices[i].prize.maxValue);
        }
    }

    private void SetCurrentWheel()
    {
        if (_uiManager.IsSuperSafeZone()) currentWheel = wheels[2];
        else if (_uiManager.IsSafeZone()) currentWheel = wheels[1];
        else currentWheel = wheels[0];
    }

    public void SetWheel()
    {
        SetCurrentWheel();

        wheelCircle.sprite = currentWheel.wheelSprite;
        indicator.sprite = currentWheel.indicatorSprite;

        SelectRandomPrizes();
        Generate();
        CalculateWeightsAndIndices();
    }


    public void Spin()
    {
        if (isSpinning) return;
        
        isSpinning = true;
        OnSpinStartEvent?.Invoke();

        var index = GetRandomPieceIndex();
        var piece = slices[index];

        if (piece.prize.chance == 0 && nonZeroChancesIndices.Count != 0)
        {
            index = nonZeroChancesIndices[Random.Range(0, nonZeroChancesIndices.Count)];
            piece = slices[index];
        }

        var angle = -(pieceAngle * index);
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
                if (diff >= pieceAngle)
                {
                    prevAngle = currentAngle;
                    isIndicatorOnTheLine = !isIndicatorOnTheLine;
                }

                currentAngle = wheelCircle.transform.eulerAngles.z;
            })
            .OnComplete(() =>
            {
                isSpinning = false;
                OnSpinEndEvent?.Invoke(piece);
            });
    }

    private void OnSpinStart()
    {
        
    }

    private void OnSpinEnd(InventorySlot item)
    {
        //SetWheel();
    }

    private void OnEnable()
    {
        OnSpinStartEvent += OnSpinStart;
        OnSpinEndEvent += OnSpinEnd;
    }
    
    private void OnDisable()
    {
        OnSpinStartEvent -= OnSpinStart;
        OnSpinEndEvent -= OnSpinEnd;
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
    }
}