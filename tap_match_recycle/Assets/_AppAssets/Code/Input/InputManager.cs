using System;
using _AppAssets.Code.Utilities;
using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace _AppAssets.Code.Input
{
    public abstract class InputManager : MonoBehaviour
    {
        public event Action<GameObject> OnItemTapped;

        protected bool _inputBlocked;

        protected Camera _mainCamera;

        public void Initialize()
        {
            _mainCamera = Camera.main;
            _inputBlocked = true;
        }
        
        public abstract void HandleInput();

        private void Update()
        {
            if (_inputBlocked)
            {
                return;
            }
            
            HandleInput();
        }

        public void ToggleInputBlocked(bool blocked)
        {
            _inputBlocked = blocked;
        }

        protected void CastRay()
        {
            if (_mainCamera.RayCastToScreenPosition(UnityInput.mousePosition, out GameObject hitObject))
            {
                RaiseOnItemTappedEvent(hitObject);
            }
        }
        
        protected void RaiseOnItemTappedEvent(GameObject hitObject)
        {
            OnItemTapped?.Invoke(hitObject);
        }
    }
    
    
    public class EditorInputManager : InputManager
    {
        public override void HandleInput()
        {
            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                CastRay();
            }
        }
    }

    public class MobileInputManager : InputManager
    {
        public override void HandleInput()
        {
            if (UnityInput.touches.Length > 0)
            {
                Touch mainTouch = UnityInput.GetTouch(0);
                
                if (mainTouch.phase == TouchPhase.Ended)
                {
                    Vector3 inputPos = mainTouch.position;
                    
                    CastRay();
                }
            }
        }
    }
}