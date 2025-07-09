using System;
using GameSession.Cars.Enums;
using UnityEngine;

namespace GameSession.Cars.Data
{
    [Serializable]
    public class CarDescriptor
    {
        [field: SerializeField] public CarType Type { get; private set; }
        [field: SerializeField] public Sprite HorizontalSprite { get; private set; }
        [field: SerializeField] public Sprite VerticalSprite { get; private set; }
        [field: SerializeField] public float Cost { get; private set; }
        [field: SerializeField] public float SalePrice { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float ProductCapacity { get; private set; }
    }
}