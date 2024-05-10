using System.Collections.Generic;
using CarsSystem.Data;
using UnityEngine;

namespace CarsSystem.Storages
{
    [CreateAssetMenu(fileName = nameof(CarsStorage), menuName = "CarsSystem/CarsStorage")]
    public class CarsStorage : ScriptableObject
    {
        [field: SerializeField] public List<CarDescriptor> CarDescriptors { get; private set; }
    }
}
