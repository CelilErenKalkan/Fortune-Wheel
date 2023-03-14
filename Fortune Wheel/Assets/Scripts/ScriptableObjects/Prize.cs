using UnityEngine;

namespace ScriptableObjects
{
    public enum Rarity
    {
        Standard,
        Silver,
        Gold
    }

    public enum PrizeType
    {
        Gold,
        Money,
        Item
    }
    
    [CreateAssetMenu(fileName = "newItem",menuName = "Item", order = 0)]
    public class Prize : ScriptableObject
    {
        public string itemName;
        public Sprite icon;
        public PrizeType prizeType;
        
        [Tooltip ("Reward Amount Ratio")] public int ratio ;
        [Tooltip ("Reward Amount Maximum Value")] public int maxValue ;
        [Tooltip ("Item Rarity")] public Rarity rarity ;

        [Tooltip ("Probability in %")] 
        [Range (0f, 100f)] 
        public float chance = 100f ;
        
        [HideInInspector] public int index ;
        [HideInInspector] public double weight = 0f ;
    }
}
