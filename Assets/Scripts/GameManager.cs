using LootLocker.Requests;
using UnityEngine;

// Persistant game object source: https://pavcreations.com/data-persistence-or-how-to-save-load-game-data-in-unity/
// Lootlocker source: https://docs.lootlocker.com/players/authentication/guest-login

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

    private void Start()
    {
        SetupLootLockerGuestSession();
    }
    #endregion

    private static void SetupLootLockerGuestSession()
    {
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (!response.success)
            {
                Debug.Log("Error starting Lootlocker session");
                return;
            }

            Debug.Log("Successfully started LootLocker session");
        });
    }
}
