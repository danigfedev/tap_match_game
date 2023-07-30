using System;
using _AppAssets.Code.Utilities;
using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace _AppAssets.Code.Input
{
    public abstract class InputManager<T> where T : MonoBehaviour, IPoolable
    {
        public event Action<T> OnItemTapped;

        protected bool _inputBlocked;

        protected Camera _mainCamera;

        public InputManager()
        {
            _mainCamera = Camera.main;
        }
        
        public abstract void HandleInput();
        
        public void ToggleInputBlocked(bool blocked)
        {
            _inputBlocked = blocked;
        }

        protected void RaiseOnItemTappedEvent(T item)
        {
            OnItemTapped?.Invoke(item);
        }
    }
    
    
    public class EditorInputManager<T> : InputManager<T> where T : MonoBehaviour, IPoolable
    {
        public override void HandleInput()
        {
            if (_inputBlocked)
            {
                return;
            }
            
            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                if (_mainCamera.RayCastToScreenPosition<T>(UnityInput.mousePosition, out T hitObject))
                {
                    RaiseOnItemTappedEvent(hitObject);
                }
            }
        }
    }

    public class MobileInputManager<T> : InputManager<T> where T : MonoBehaviour, IPoolable
    {
        public override void HandleInput()
        {
            if (_inputBlocked)
            {
                return;
            }
            
            if (UnityInput.touches.Length > 0)
            {
                Touch mainTouch = UnityInput.GetTouch(0);
                
                if (mainTouch.phase == TouchPhase.Ended)
                {
                    Vector3 inputPos = mainTouch.position;
                    
                    if (_mainCamera.RayCastToScreenPosition<T>(inputPos, out T hitObject))
                    {
                        RaiseOnItemTappedEvent(hitObject);
                    }
                }
            }
        }
    }
}