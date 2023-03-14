using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoSingleton<InventoryManager>
{
    public List<InventorySlot> inventory;
    [SerializeField] private Transform slotsParent;
    [SerializeField] private GameObject showInventoryButton, inventoryObject;
    private List<Image> slotImages = new List<Image>();
    private List<TMP_Text> itemAmountList = new List<TMP_Text>();

    // Start is called before the first frame update
    private void Start()
    {
        Inventory.LoadInventory();
        inventory = Inventory.Slots;
        SetSlots();
    }

    public void ShowInventory(bool state)
    {
        SetInventoryList();
        
        if (state)
        {
            showInventoryButton.SetActive(!state);
            inventoryObject.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0.1f).OnComplete(()=> inventoryObject.SetActive(state));
            inventoryObject.transform.DOScale(new Vector3(1, 1, 1), 0.5f).SetEase(Ease.OutBack);
        }
        else
        {
            inventoryObject.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0.5f)
                .SetEase(Ease.InBack).OnComplete(()=>
                {
                    inventoryObject.SetActive(state);
                    showInventoryButton.SetActive(!state);
                });
            inventoryObject.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f);
        }
    }

    public void SetInventoryList()
    {
        for (var i = 0; i < slotsParent.childCount; i++)
        {
            if (i < inventory.Count)
            {
                slotImages[i].sprite = inventory[i].prize.icon;
                slotImages[i].enabled = true;
                itemAmountList[i].text = Inventory.SetAmountText(inventory[i].amount);
            }
            else
            {
                slotImages[i].sprite = null;
                slotImages[i].enabled = false;
                itemAmountList[i].text = "";
            }
        }
    }

    private void SetSlots()
    {
        for (var i = 0; i < slotsParent.childCount; i++)
        {
            if (slotsParent.GetChild(i).TryGetComponent(out Image image)) slotImages.Add(image);
            image.enabled = false;
            if (slotsParent.GetChild(i).GetChild(0).TryGetComponent(out TMP_Text text)) itemAmountList.Add(text);
        }
    }
}
