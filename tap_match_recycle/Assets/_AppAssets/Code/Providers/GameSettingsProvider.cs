using _AppAssets.Code.Settings;
using UnityEngine;

namespace _AppAssets.Code
{
    [CreateAssetMenu(fileName = "GameSettingsProvider", menuName = "TapMatchRecycle/Providers/GameSettingsProvider")]
    public class GameSettingsProvider : ScriptableObject
    {
        public GameSettings GameSettings;
        public DisplaySettings DisplaySettings;
    }
}