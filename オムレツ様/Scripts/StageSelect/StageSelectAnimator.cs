using System;
using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

namespace Ko.StageSelect
{
    public sealed class StageSelectAnimator : MonoBehaviour
    {
        [Header("Stage Hover Animation")]
        [SerializeField, Min(0.01f)] private float _hoverDuration = 0.4f;

        [SerializeField, Min(1f)] private float _hoverScaleMultiplier = 1.2f;
        [SerializeField] private Vector3 _spinAxis = Vector3.up;

        [Header("Stage Click Animation")]
        [SerializeField] private CinemachineCamera _mainCamera;
        [SerializeField, Min(0.01f)] private float _cameraMoveDuration = 2f;
        [SerializeField] private float _selectedCameraHeight = 5f;
        [SerializeField] private float _selectedCameraZOffset = -1f;

        [Header("Locked Stage Click Animation")]
        [SerializeField, Min(0.01f)] private float _lockedStageClickDuration = 0.05f;
        [SerializeField, Min(1f)] private float _lockedStageClickScaleMultiplier = 1.2f;
        [SerializeField, Min(0f)] private float _lockedStageShakeDistance = 0.1f;
        [SerializeField, Min(1)] private int _lockedStageShakeCount = 4;

        [Header("Easing")]
        [SerializeField] private Ease _scaleEase = Ease.OutBack;

        [SerializeField] private Ease _rotationEase = Ease.OutCubic;
        [SerializeField] private Ease _textEase = Ease.OutCubic;
        [SerializeField] private Ease _fontSizeEase = Ease.OutCubic;
        [SerializeField] private Ease _cameraEase = Ease.InOutCubic;
        [SerializeField] private Ease _confirmButtonEase = Ease.InOutCubic;
        [SerializeField] private Ease _lockedStageClickEase = Ease.Linear;

        [Header("Stage Text Animation")]
        [SerializeField] private Vector3 _hoverTextPosition = new Vector3(0f, 0.5f, 4f);
        [SerializeField, Min(0f)] private float _hoverFontSize = 10f;

        [Header("Confirm Button Animation")]
        [SerializeField] private GameObject _confirmButton;
        [SerializeField] private Vector3 _confirmButtonDefaultPosition = new Vector3(1920f, 0f, 0f);
        [SerializeField] private Vector3 _confirmButtonPosition = new Vector3(0f, 0f, 0f);
        [SerializeField, Min(0.01f)] private float _confirmButtonMoveDuration = 0.3f;


        private MotionHandle _cameraMoveHandle;
        private MotionHandle _confirmButtonMoveHandle;
        private Vector3 _defaultCameraPosition;
        private readonly Dictionary<StageSelectItem, ItemAnimationState> _animationStates = new();

        public void PlayPointerEnterAnimation(StageSelectItem item)
        {
            if (!TryGetAnimationState(item, out ItemAnimationState state))
            {
                return;
            }

            PlayHoverMotion(state, targetSpinAngle: 360f, targetScaleMultiplier: _hoverScaleMultiplier, isHovered: true);
        }

        public void PlayPointerExitAnimation(StageSelectItem item)
        {
            if (!TryGetAnimationState(item, out ItemAnimationState state))
            {
                return;
            }

            PlayHoverMotion(state, targetSpinAngle: 0f, targetScaleMultiplier: 1f, isHovered: false);
        }

        public void PlayCameraFocusAnimation(StageSelectItem item)
        {
            if (item == null || _mainCamera == null)
            {
                return;
            }
            if (_cameraMoveHandle.IsActive())
            {
                _cameraMoveHandle.Cancel();
            }

            Vector3 stagePosition = item.transform.position;
            _defaultCameraPosition = _mainCamera.transform.position;
            Vector3 targetPosition = new Vector3(stagePosition.x, _selectedCameraHeight, stagePosition.z + _selectedCameraZOffset);

            _cameraMoveHandle = LMotion
                .Create(
                    _mainCamera.transform.position,
                    targetPosition,
                    _cameraMoveDuration)
                .WithEase(_cameraEase)
                .BindToPosition(_mainCamera.transform)
                .AddTo(_mainCamera.transform);
        }

        public void PlayCameraReturnAnimation()
        {
            if (_mainCamera == null)
            {
                return;
            }
            if (_cameraMoveHandle.IsActive())
            {
                _cameraMoveHandle.Cancel();
            }

            Vector3 targetPosition = _defaultCameraPosition;

            _cameraMoveHandle = LMotion
                .Create(
                    _mainCamera.transform.position,
                    targetPosition,
                    _cameraMoveDuration)
                .WithEase(_cameraEase)
                .BindToPosition(_mainCamera.transform)
                .AddTo(_mainCamera.transform);
        }

        public async UniTask PlaySelectedAnimation(StageSelectItem item)
        {
            PlayPointerEnterAnimation(item); // 最終的なホバー状態まで進めるための処理
            PlayCameraFocusAnimation(item);
            await UniTask.Delay(TimeSpan.FromSeconds(_cameraMoveDuration));
            PlayConfirmButtonEnterAnimation();
        }

        private void PlayHoverMotion(ItemAnimationState state, float targetSpinAngle, float targetScaleMultiplier, bool isHovered)
        {
            state.CancelMotions();

            // ステージの見た目のアニメーション
            float remainingRate = Mathf.Abs(targetSpinAngle - state.SpinAngle) / 360f;

            float duration = Mathf.Max(0.01f, _hoverDuration * remainingRate);

            Vector3 targetScale = state.DefaultScale * targetScaleMultiplier;

            state.IsHovered = isHovered;

            state.ScaleHandle = LMotion
                .Create(
                    state.Target.localScale,
                    targetScale,
                    duration)
                .WithEase(_scaleEase)
                .BindToLocalScale(state.Target)
                .AddTo(state.Target);

            state.RotationHandle = LMotion
                .Create(
                    state.SpinAngle,
                    targetSpinAngle,
                    duration)
                .WithEase(_rotationEase)
                .Bind(
                    state,
                    static (angle, animationState) =>
                    {
                        animationState.ApplyRotation(angle);
                    })
                .AddTo(state.Target);

            // ステージ名のアニメーション
            if (state.StageNameText == null || state.StageNameRectTransform == null)
            {
                return;
            }
            Vector3 targetTextPosition = isHovered ? _hoverTextPosition : state.DefaultTextPosition;
            float targetFontSize = isHovered ? _hoverFontSize : state.DefaultFontSize;
            state.TextPositionHandle = LMotion
                .Create(
                    state.StageNameRectTransform.anchoredPosition3D,
                    targetTextPosition,
                    duration)
                .WithEase(_textEase)
                .BindToAnchoredPosition3D(
                    state.StageNameRectTransform)
                .AddTo(state.StageNameRectTransform);

            state.FontSizeHandle = LMotion
                .Create(
                    state.StageNameText.fontSize,
                    targetFontSize,
                    duration)
                .WithEase(_fontSizeEase)
                .BindToFontSize(state.StageNameText)
                .AddTo(state.StageNameText);
        }

        public void PlayLockedStageClickedAnimation(StageSelectItem item)
        {
            if (!TryGetAnimationState(item, out ItemAnimationState state))
            {
                return;
            }

            if (state.LockedClickHandle.IsActive())
            {
                state.LockedClickHandle.Cancel();
            }

            if (state.ScaleHandle.IsActive())
            {
                state.ScaleHandle.Cancel();
            }

            Vector3 baseScale = state.DefaultScale * (state.IsHovered ? _hoverScaleMultiplier : 1f);

            Vector3 basePosition = state.DefaultLocalPosition;
            Transform target = state.Target;

            //連打してもバグが起きないよう基準に戻す
            target.localScale = baseScale;
            target.localPosition = basePosition;

            state.LockedClickHandle = LMotion
                .Create(0f, 1f, _lockedStageClickDuration)
                .WithEase(_lockedStageClickEase)
                .Bind(progress =>
                {
                    float scalePusle = Mathf.Sin(progress * Mathf.PI);
                    float shake = Mathf.Sin(progress * Mathf.PI * 2f * _lockedStageShakeCount) * _lockedStageShakeDistance * (1f - progress);
                    target.localScale = Vector3.LerpUnclamped(baseScale, baseScale * _lockedStageClickScaleMultiplier, scalePusle);
                    target.localPosition = basePosition + Vector3.right * (_lockedStageShakeDistance * shake);
                })
                .AddTo(target);
        }

        public void InitializeConfirmButton()
        {
            if (_confirmButton == null)
            {
                return;
            }

            _confirmButton.transform.localPosition = _confirmButtonDefaultPosition;
        }

        public void PlayConfirmButtonEnterAnimation()
        {

            if (_confirmButton == null)
            {
                return;
            }

            _confirmButton.SetActive(true);

            if (_confirmButtonMoveHandle.IsActive())
            {
                _confirmButtonMoveHandle.Cancel();
            }

            _confirmButtonMoveHandle = LMotion
                .Create(
                    _confirmButton.transform.localPosition,
                    _confirmButtonPosition,
                    _confirmButtonMoveDuration)
                .WithEase(_confirmButtonEase)
                .BindToLocalPosition(_confirmButton.transform)
                .AddTo(_confirmButton.transform);
        }

        public void PlayConfirmButtonExitAnimation()
        {
            if (_confirmButton == null)
            {
                return;
            }

            if (_confirmButtonMoveHandle.IsActive())
            {
                _confirmButtonMoveHandle.Cancel();
            }

            _confirmButtonMoveHandle = LMotion
                .Create(
                    _confirmButton.transform.localPosition,
                    _confirmButtonDefaultPosition,
                    _confirmButtonMoveDuration)
                .WithEase(_confirmButtonEase)
                .BindToLocalPosition(_confirmButton.transform)
                .AddTo(_confirmButton.transform);

            _confirmButton.SetActive(false);
        }

        private bool TryGetAnimationState(StageSelectItem item, out ItemAnimationState state)
        {
            state = null;

            if (item == null || item.ViewRoot == null)
            {
                return false;
            }

            if (_animationStates.TryGetValue(item, out state))
            {
                return true;
            }

            state = new ItemAnimationState(item.ViewRoot, item.StageNameText, _spinAxis);

            _animationStates.Add(item, state);
            return true;
        }

        private void OnDestroy()
        {
            if (_cameraMoveHandle.IsActive())
            {
                _cameraMoveHandle.Cancel();
            }

            foreach (ItemAnimationState state in _animationStates.Values)
            {
                state.CancelMotions();
            }

            _animationStates.Clear();
        }

        private sealed class ItemAnimationState
        {
            private readonly Quaternion _defaultRotation;
            private readonly Vector3 _spinAxis;

            public Transform Target { get; }
            public Vector3 DefaultScale { get; }

            public float SpinAngle { get; private set; }

            public MotionHandle ScaleHandle { get; set; }
            public MotionHandle RotationHandle { get; set; }

            public TMP_Text StageNameText { get; }
            public RectTransform StageNameRectTransform { get; }

            public Vector3 DefaultTextPosition { get; }
            public float DefaultFontSize { get; }

            public MotionHandle TextPositionHandle { get; set; }
            public MotionHandle FontSizeHandle { get; set; }

            public Vector3 DefaultLocalPosition { get; }
            public bool IsHovered { get; set; }
            public MotionHandle LockedClickHandle { get; set; }

            public ItemAnimationState(Transform target, TMP_Text stageNameText, Vector3 spinAxis)
            {
                Target = target;
                DefaultScale = target.localScale;
                _defaultRotation = target.localRotation;

                _spinAxis = spinAxis.sqrMagnitude > 0f ? spinAxis.normalized : Vector3.up;
                StageNameText = stageNameText;

                if (stageNameText != null)
                {
                    StageNameRectTransform =
                        stageNameText.rectTransform;

                    DefaultTextPosition =
                        StageNameRectTransform.anchoredPosition3D;

                    DefaultFontSize =
                        stageNameText.fontSize;
                }
                DefaultLocalPosition = target.localPosition;

            }

            public void ApplyRotation(float angle)
            {
                SpinAngle = angle;
                Target.localRotation = Quaternion.AngleAxis(angle, _spinAxis) * _defaultRotation;
            }

            public void CancelMotions()
            {
                if (ScaleHandle.IsActive())
                {
                    ScaleHandle.Cancel();
                }

                if (RotationHandle.IsActive())
                {
                    RotationHandle.Cancel();
                }

                if (TextPositionHandle.IsActive())
                {
                    TextPositionHandle.Cancel();
                }

                if (FontSizeHandle.IsActive())
                {
                    FontSizeHandle.Cancel();
                }
            }
        }
    }
}