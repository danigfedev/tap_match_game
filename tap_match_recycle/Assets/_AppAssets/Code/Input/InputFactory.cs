using UnityEngine;

namespace _AppAssets.Code.Input
{
    public class InputFactory
    {
        public static InputManager CreateInputManager(GameObject manager, RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.LinuxEditor:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                {
                    var a = manager.AddComponent<EditorInputManager>();
                    return a;
                }
                case RuntimePlatform.Android:
                case RuntimePlatform.IPhonePlayer:
                {
                    return manager.AddComponent<MobileInputManager>();
                }
                default:
                {
                    Debug.LogWarning("Platform not supported");
                    return null;
                }
            }
        }
    }
}