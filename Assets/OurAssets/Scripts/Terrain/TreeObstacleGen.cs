using UnityEngine;
using Unity.AI.Navigation;

[RequireComponent(typeof(Terrain), typeof(NavMeshSurface))]
public class TreeObstacleGen : MonoBehaviour
{
    #if UNITY_EDITOR
        [HideInInspector]
        public bool generateTrees = false;
    #endif
}
