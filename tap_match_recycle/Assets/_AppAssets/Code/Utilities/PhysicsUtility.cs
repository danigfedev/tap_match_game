using UnityEngine;

namespace _AppAssets.Code.Utilities
{
    public static class PhysicsUtility
    {
        public static bool RayCastToScreenPosition(this Camera mainCamera, Vector3 screenPosition, out GameObject hitObject)
        {
            Ray ray = mainCamera.ScreenPointToRay(screenPosition);

            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            
            hitObject = hit.collider?.gameObject;
            
            return hit.collider != null;
        }
        
        public static bool RayCastToScreenPosition<T>(this Camera mainCamera, Vector3 screenPosition, out T hitObject)
        {
            Ray ray = mainCamera.ScreenPointToRay(screenPosition);

            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider == null)
            {
                hitObject = default;
                return false;
            }

            hitObject = hit.collider.GetComponent<T>();

            if (hitObject == null)
            {
                return false;
            }
            
            return true;
        }
    }
}