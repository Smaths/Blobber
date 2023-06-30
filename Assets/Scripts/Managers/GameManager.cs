using UnityEngine;

// Persistant game object source: https://pavcreations.com/data-persistence-or-how-to-save-load-game-data-in-unity/
// Script execution order modified.

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        #region Lifecycle
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this); // Persist the object on scene unload
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion
    }
}
