using System.Collections.Generic;
using UnityEngine;

namespace PathBuilding
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PathPointDescriptor : MonoBehaviour
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public List<int> NextPointsIds { get; private set; }
        [field: SerializeField] public bool IsStartingPoint { get; private set; }
        [field: SerializeField] public bool IsEndingPoint { get; private set; }
    }
}
