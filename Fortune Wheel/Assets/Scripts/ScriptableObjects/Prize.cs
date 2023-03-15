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
        
        public int ratio ;
        public int maxValue ;
        public Rarity rarity ;
        
        [Range (0f, 100f)] 
        public float chance = 100f ;
        
        [HideInInspector] public int index ;
        [HideInInspector] public double weight = 0f ;
    }
}
