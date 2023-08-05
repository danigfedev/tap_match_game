using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EG.EndlessShapes.UI
{
    public class AnimatedButton : Button
    {
        private enum AnimationDirections
        {
            ScaleUp,
            ScaleDown,
        }

        [SerializeField] private Transform _animationTarget;
        [SerializeField] private float _shrinkScale = 0.85f;
        [SerializeField] private float _animationSpeed = 2f;

        private float _initialScale;
        private bool _isDirty;
        private Coroutine _animationCoroutine;
        private float _elapsedTime = 0;

        protected override void Awake()
        {
            if(_animationTarget == null)
            {
                _animationTarget = transform;
            }
            _initialScale = _animationTarget.localScale.x;

            base.Awake();
        }
        
        protected override void OnDisable()
        {
            if (!_isDirty)
            {
                return;
            }

            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
                _animationCoroutine = null;
            }

            _animationTarget.localScale = _initialScale * Vector3.one;
            
            _isDirty = false;
            
            base.OnDisable();
        }
        
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (this.interactable)
            {
                PlayAnimation(AnimationDirections.ScaleDown);
            }

            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (this.interactable)
            {
                PlayAnimation(AnimationDirections.ScaleUp);
            }
            
            base.OnPointerUp(eventData);
        }

        private void PlayAnimation(AnimationDirections direction)
        {
            _isDirty = true;

            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }

            _animationCoroutine = StartCoroutine(AnimateButton(direction));
        }

        private IEnumerator AnimateButton(AnimationDirections direction)
        {
            float start, end;

            switch (direction)
            {
                case AnimationDirections.ScaleUp: 
                    start = _shrinkScale;
                    end = _initialScale;
                    break;
                case AnimationDirections.ScaleDown:
                    start = _initialScale;
                    end = _shrinkScale;
                    break;
                default:
                    start = _shrinkScale;
                    end = _initialScale;
                    break;
            }

            _elapsedTime = 0;
            float newScale = start;

            while (_elapsedTime <= 1)
            {
                newScale = Mathf.Lerp(start, end, _elapsedTime);
                _animationTarget.localScale = newScale * Vector3.one;

                yield return null;

                _elapsedTime += _animationSpeed * Time.deltaTime;
            }

            //The complete animation cycle has ended (scale down -> up)
            if (direction == AnimationDirections.ScaleUp)
            {
                _isDirty = false;
            }

            _animationCoroutine = null;
        }
    }
}