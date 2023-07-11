using UnityEngine;

// Persistant game object source: https://pavcreations.com/data-persistence-or-how-to-save-load-game-data-in-unity/
// Script execution order modified.

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        #region Lifecycle
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        #endregion
    }
}
