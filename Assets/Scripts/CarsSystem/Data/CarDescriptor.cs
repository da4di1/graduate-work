using System;
using CarsSystem.Enums;
using UnityEngine;

namespace CarsSystem.Data
{
    [Serializable]
    public class CarDescriptor
    {
        [field: SerializeField] public CarType CarType { get; private set; }
        [field: SerializeField] public Sprite HorizontalSprite { get; private set; }
        [field: SerializeField] public Sprite VerticalSprite { get; private set; }
        [field: SerializeField] public float Price { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float Capacity { get; private set; }
    }
}
