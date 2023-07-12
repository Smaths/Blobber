using Sirenix.OdinInspector;

namespace Utility
{
    // Persistant game object source: https://pavcreations.com/data-persistence-or-how-to-save-load-game-data-in-unity/
    public class Persistence : Singleton<Persistence>
    {
        [TitleGroup("Keep the game object and all attached scripts alive between scenes.")]
        protected override void Awake()
        {
            base.Awake();

            DontDestroyOnLoad(this); // Persist the object on scene unload
        }
    }
}