using Sirenix.OdinInspector;
using UnityEngine;

namespace Environment
{
    public class ObstacleManager : MonoBehaviour
    {
        [Title("Obstacle Manager", "Operates on child game objects with the 'RandomMeshRack'")]
        [SerializeField] private RandomMeshRack[] _obstacles;

        private void OnValidate()
        {
            FindMeshRacksInChildren();
        }

        [Button("Find Obstacles", ButtonSizes.Medium, Icon = SdfIconType.Search)]
        private void FindMeshRacksInChildren()
        {
            RandomMeshRack[] foundObjects = GetComponentsInChildren<RandomMeshRack>();
            _obstacles = foundObjects;
        }

        [Button("Randomize All", ButtonSizes.Medium, Icon = SdfIconType.Dice6Fill)]
        private void RandomizeObstacleMeshes()
        {
            foreach (RandomMeshRack obstacle in _obstacles)
            {
                obstacle.RandomizeAll();
            }
        }

        [Button(ButtonSizes.Medium, Icon = SdfIconType.ArrowClockwise)]
        private void RandomizeRotation()
        {
            foreach (RandomMeshRack obstacle in _obstacles)
            {
                obstacle.RandomizeRotation();
            }
        }
    }
}
