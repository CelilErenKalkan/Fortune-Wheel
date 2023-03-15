using System.Collections.Generic;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    private CurrencyManager currencyManager;
    
    [SerializeField] private FortuneWheel fortuneWheel;

    [Header("PANELS:")]
    [SerializeField] private GameObject spinEndPanel;
    [SerializeField] private GameObject bombPanel, prizePanel;
    [SerializeField] private Transform prizeListParent;

    [Header("ITEMS:")]
    public Prize bomb;
    public List<InventorySlot> obtainedPrizes;
    
    [Header("TEXTS:")]
    [SerializeField] private TMP_Text zoneText;
    [SerializeField] private TMP_Text prizeAmount, prizeName, moneyText, goldText, reviveText;
    
    [Header("IMAGES:")]
    [SerializeField] private Image prizeImage;
    [SerializeField] private Image prizeFrame;
    [SerializeField] private Image spinButton;

    [Header("SPRITES:")]
    [SerializeField] private Sprite[] itemFrames = new Sprite[3];
    [SerializeField] private Sprite[] spinButtonSprites;
    
    [HideInInspector] public int zoneNo;

    private List<Image> prizeListImages = new List<Image>();
    private List<TMP_Text> prizeAmounts = new List<TMP_Text>();

    
    private const int RevivePrice = 10;
    
    public bool IsSafeZone() => zoneNo % 5 == 0;
    public bool IsSuperSafeZone() => zoneNo % 30 == 0;

    // Start is called before the first frame update
    private void Start()
    {
        zoneNo = 1;
        currencyManager = CurrencyManager.Instance;
        SetZoneNoUI(true);
        SetPrizeList();
        UpdateCurrencyUI();
    }

    private void AddNewItemToTheList(InventorySlot slot)
    {
        foreach (var prize in obtainedPrizes)
        {
            if (prize.prize != slot.prize) continue;
            
            prize.amount += slot.amount;
            return;
        }

        var newItem = new InventorySlot
        {
            prize = slot.prize,
            amount = slot.amount
        };
        obtainedPrizes.Add(newItem);
    }

    private void SetPrizeList()
    {
        for (var i = 0; i < prizeListParent.childCount; i++)
        {
            if (prizeListParent.GetChild(i).TryGetComponent(out Image image)) prizeListImages.Add(image);
            image.enabled = false;
            if (prizeListParent.GetChild(i).GetChild(0).TryGetComponent(out TMP_Text text)) prizeAmounts.Add(text);
        }
    }

    private void UpdatePrizesList()
    {
        for (var i = 0; i < prizeListParent.childCount; i++)
        {
            if (i < obtainedPrizes.Count)
            {
                prizeListImages[i].sprite = obtainedPrizes[i].prize.icon;
                prizeListImages[i].enabled = true;
                prizeAmounts[i].text = Inventory.Inventory.SetAmountText(obtainedPrizes[i].amount);
            }
            else
            {
                prizeListImages[i].sprite = null;
                prizeListImages[i].enabled = false;
                prizeAmounts[i].text = "";
            }
        }
    }

    private void UpdateCurrencyUI()
    {
        moneyText.text = currencyManager.currency.money + "$";
        goldText.text = currencyManager.currency.gold.ToString();
    }

    public void ResetGame()
    {
        zoneNo = 1;
        bombPanel.SetActive(false);
        spinEndPanel.SetActive(false);
        StartANewGame();
    }
    
    public void StartANewGame()
    {
        prizePanel.SetActive(false);
        spinEndPanel.SetActive(false);
        fortuneWheel.SetWheel();
        SetZoneNoUI(true);
    }

    private void SetZoneNoUI(bool isSpin)
    {
        if (isSpin)
        {
            if (IsSuperSafeZone())
            {
                zoneText.text = "SUPER SPIN";
                spinButton.sprite = spinButtonSprites[1];
            }
            else if (IsSafeZone())
            {
                zoneText.text = "SAFE SPIN";
                spinButton.sprite = spinButtonSprites[1];
            }
            else
            {
                zoneText.text = "SPIN";
                spinButton.sprite = spinButtonSprites[0];
            }
        }
        else
        {
            zoneText.text = zoneNo.ToString();
        }
    }

    public void CollectAllRewards()
    {
        foreach (var slot in obtainedPrizes)
        {
            switch (slot.prize.prizeType)
            {
                case PrizeType.Money:
                    CurrencyManager.onMoneyUpdate?.Invoke(slot.amount);
                    UpdateCurrencyUI();
                    break;
                case PrizeType.Gold:
                    CurrencyManager.onGoldUpdate?.Invoke(slot.amount);
                    UpdateCurrencyUI();
                    break;
                case PrizeType.Item:
                default:
                    Inventory.Inventory.AddNewItem(slot);
                    break;
            }
        }

        ResetGame();
    }

    public void Revive()
    {
        if (currencyManager.currency.gold < RevivePrice) return;
        
        bombPanel.SetActive(false);
        CurrencyManager.onGoldUpdate?.Invoke(RevivePrice * -1);
        UpdateCurrencyUI();
    }

    private void SetPrizeCard(InventorySlot slot)
    {
        prizeFrame.sprite = slot.prize.rarity switch
        {
            Rarity.Standard => itemFrames[0],
            Rarity.Silver => itemFrames[1],
            Rarity.Gold => itemFrames[2],
            _ => prizeFrame.sprite
        };

        prizePanel.SetActive(true);
        prizeImage.sprite = slot.prize.icon;
        prizeName.text = slot.prize.itemName;
        prizeAmount.text ="x" + slot.amount;
    }

    private void OnSpinStart()
    {
        SetZoneNoUI(false);
    }

    private void OnSpinEnd(InventorySlot slot)
    {
        if (slot.prize == bomb)
        {
            reviveText.text = RevivePrice.ToString();
            bombPanel.SetActive(true);
        }
        else if (IsSuperSafeZone() || IsSafeZone() && zoneNo != 1)
        {
            spinEndPanel.SetActive(true);
            SetZoneNoUI(true);
            UpdatePrizesList();
        }
        else
        {
            SetPrizeCard(slot);
            AddNewItemToTheList(slot);
        }
        
        zoneNo++;
    }

    private void OnEnable()
    {
        FortuneWheel.onSpinEndEvent += OnSpinEnd;
        FortuneWheel.onSpinStartEvent += OnSpinStart;
    }
    
    private void OnDisable()
    {
        FortuneWheel.onSpinEndEvent -= OnSpinEnd;
        FortuneWheel.onSpinStartEvent -= OnSpinStart;
    }
}