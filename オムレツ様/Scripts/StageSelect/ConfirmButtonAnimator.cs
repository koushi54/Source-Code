using System;
using UnityEngine;
using UnityEngine.EventSystems;
using LitMotion;
using LitMotion.Extensions;

namespace Ko.StageSelect
{
    public class ConfirmButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("アニメーション設定")]
        [SerializeField] private float _hoverScaleMultiplier = 1.05f;
        [SerializeField] private float _hoverDuration = 0.5f;
        [SerializeField] private float _hoverHeight = 10f;
        [SerializeField] private float _hoverReturnDuration = 0.15f;

        private GameObject _confirmButton;
        private Vector3 _defaultScale;
        private Vector3 _defaultPosition;
        private MotionHandle _scaleHandle;
        private MotionHandle _floatingHandle;

        void Awake()
        {
            _defaultPosition = transform.localPosition;
            _defaultScale = transform.localScale;
            _confirmButton = gameObject;
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            PlayScaleAnimation(_defaultScale * _hoverScaleMultiplier, _hoverDuration);
            PlayFloatingAnimation();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PlayScaleAnimation(_defaultScale, _hoverReturnDuration);
            StopFloatingAnimation();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            PlayScaleAnimation(_defaultScale, _hoverReturnDuration);
            StopFloatingAnimation();
        }

        private void PlayScaleAnimation(Vector3 targetScale, float duration)
        {
            if (_scaleHandle != null && _scaleHandle.IsActive())
            {
                _scaleHandle.Cancel();
            }
            _scaleHandle = LMotion
                .Create(transform.localScale, targetScale, duration)
                .WithEase(Ease.InOutSine)
                .BindToLocalScale(transform)
                .AddTo(transform);
        }

        private void PlayFloatingAnimation()
        {
            if (_floatingHandle != null && _floatingHandle.IsActive())
            {
                _floatingHandle.Cancel();
            }
            Vector3 floatingTargetPosition = _defaultPosition + Vector3.up * _hoverHeight;

            _floatingHandle = LMotion
                .Create(_confirmButton.transform.localPosition, floatingTargetPosition, _hoverDuration)
                .WithEase(Ease.InOutSine)
                .WithLoops(-1, LoopType.Yoyo)
                .BindToLocalPosition(_confirmButton.transform)
                .AddTo(_confirmButton.transform);
        }

        private void StopFloatingAnimation()
        {
            if (_floatingHandle != null && _floatingHandle.IsActive())
            {
                _floatingHandle.Cancel();
            }

            _floatingHandle = LMotion
                .Create(_confirmButton.transform.localPosition, _defaultPosition, _hoverReturnDuration)
                .WithEase(Ease.OutCubic)
                .BindToLocalPosition(_confirmButton.transform)
                .AddTo(_confirmButton.transform);

        }

        private void OnDestroy()
        {
            if (_scaleHandle != null && _scaleHandle.IsActive())
            {
                _scaleHandle.Cancel();
            }

            if (_floatingHandle != null && _floatingHandle.IsActive())
            {
                _floatingHandle.Cancel();
            }

            _confirmButton.transform.localScale = _defaultScale;
            _confirmButton.transform.localPosition = _defaultPosition;
        }
    }
}