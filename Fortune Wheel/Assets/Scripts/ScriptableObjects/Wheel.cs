using UnityEngine;

namespace ScriptableObjects
{
    public enum WheelType
    {
        Bronze,
        Silver,
        Gold
    }

    [CreateAssetMenu(fileName = "newWheel",menuName = "Wheel", order = 0)]
    public class Wheel : ScriptableObject
    {
        public WheelType wheelType;
        public Sprite wheelSprite;
        public Sprite indicatorSprite;
        public Prize[] prizes;
    }
}