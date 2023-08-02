using UnityEngine;

namespace _AppAssets.Code.Input
{
    public class InputFactory
    {
        public static InputManager<T> CreateInputManager<T>(RuntimePlatform platform) where T : MonoBehaviour, IPoolable
        {
            switch (platform)
            {
                case RuntimePlatform.LinuxEditor:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                {
                    return new EditorInputManager<T>();
                }
                case RuntimePlatform.Android:
                case RuntimePlatform.IPhonePlayer:
                {
                    return new MobileInputManager<T>();
                }
                default:
                {
                    Debug.LogWarning("Platform not suppoted");
                    return null;
                }
            }
        }
    }
}