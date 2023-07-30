using _AppAssets.Code.Utilities;
using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace _AppAssets.Code.Input
{
    public abstract class InputManager<T>
    {
        public abstract bool HandleInput<T>(out T objectHit);
        
        protected bool _inputBlocked;

        public void ToggleInputBlocked(bool blocked)
        {
            _inputBlocked = blocked;
        }

        protected Camera _mainCamera;

        public InputManager()
        {
            _mainCamera = Camera.main;
        }
    }
    
    public class EditorInputManager<T> : InputManager<T> where T : MonoBehaviour, IPoolable
    {
        public override bool HandleInput<T>(out T objectHit)
        {
            objectHit = default;

            if (_inputBlocked)
            {
                return false;
            }
            
            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                if (_mainCamera.RayCastToScreenPosition<T>(UnityInput.mousePosition, out T hitObject))
                {
                    objectHit = hitObject;
                    return true;
                }
            }

            return false;
        }
    }

    public class MobileInputManager<T> : InputManager<T> where T : MonoBehaviour, IPoolable
    {
        public override bool HandleInput<T>(out T objectHit)
        {
            objectHit = default;
            
            if (_inputBlocked)
            {
                return false;
            }
            
            if (UnityInput.touches.Length > 0)
            {
                Touch mainTouch = UnityInput.GetTouch(0);
                
                if (mainTouch.phase == TouchPhase.Ended)
                {
                    Vector3 inputPos = mainTouch.position;
                    
                    if (_mainCamera.RayCastToScreenPosition<T>(inputPos, out T hitObject))
                    {
                        objectHit = hitObject;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}