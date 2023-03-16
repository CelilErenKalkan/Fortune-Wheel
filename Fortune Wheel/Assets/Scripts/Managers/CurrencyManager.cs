using System;

[Serializable]
public struct Currency
{
    public int money;
    public int gold;
}

public class CurrencyManager : MonoSingleton<CurrencyManager>
{
    public Currency currency;

    private UIManager uiManager;

    public static Action<int> onMoneyUpdate;
    public static Action<int> onGoldUpdate;
    
    private void Awake()
    {
        currency = FileHandler.ReadFromJson<Currency>("Currency.json");
    }

    private void UpdateMoney(int amount)
    {
        currency.money += amount;
        FileHandler.SaveToJson(currency, "Currency.json");
    }
    
    private void UpdateGold(int amount)
    {
        currency.gold += amount;
        FileHandler.SaveToJson(currency, "Currency.json");
    }

    private void OnEnable()
    {
        onMoneyUpdate += UpdateMoney;
        onGoldUpdate += UpdateGold;
    }
    
    private void OnDisable()
    {
        onMoneyUpdate -= UpdateMoney;
        onGoldUpdate -= UpdateGold;
    }
}
