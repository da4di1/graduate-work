using System.Collections.Generic;
using GameSession.Warehouses.Enums;
using UnityEngine;

namespace GameSession.PathBuilding
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PathPointDescriptor : MonoBehaviour
    {
        /*[field: SerializeField] public int ID { get; private set; }*/
        /*[field: SerializeField] public bool IsAvailable { get; private set; }*/
        [field: SerializeField] public List<PathPointDescriptor> NeighborPoints { get; private set; }
        [field: SerializeField] public WarehouseID AdjacentWarehouseID { get; private set; }
        [field: SerializeField] public bool AdjacentBusinessID { get; private set; }
        /*[field: SerializeField] public bool IsStartingPoint { get; private set; }
        [field: SerializeField] public bool IsEndingPoint { get; private set; }*/

        public void SetNeighborPointsActive(bool isActive, List<PathPointDescriptor> exceptionPoints = null)
        {
            foreach (var pathPoint in NeighborPoints)
            {
                if (exceptionPoints != null && exceptionPoints.Contains(pathPoint)) continue;
                
                pathPoint.gameObject.SetActive(isActive);
            }
        }
    }
}