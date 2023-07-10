using Sirenix.OdinInspector;

namespace Utility
{
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