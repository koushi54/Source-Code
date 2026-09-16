using UnityEngine;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using System;

namespace Ko.StageClear
{
    public class StageClearAnimator : MonoBehaviour
    {
        [SerializeField] StageClearContainer _container;
        [Header("カメラアニメーション")]
        [SerializeField] float _cameraRotationDuration = 3.0f;
        [SerializeField] float _cameraRotationAngle = 360f;
        [SerializeField] private Vector3 _cameraOrbitOffset = new Vector3(0f, 3f, -5f); // オムレツから見たカメラの位置

        [Header("クリアテキストアニメーション")]
        [SerializeField] float _stageClearTextAnimationDuration = 1.0f;
        [SerializeField] Vector2 _stageClearTextTargetPosition = new Vector2(400, 0);

        [Header("ボタンアニメーション")]
        [SerializeField] float _buttonAnimationDuration = 1.0f;
        [SerializeField] float _buttonAnimationDelay = 0.5f;
        [SerializeField] Vector2 _gotoStageSelectButtonTargetPosition = new Vector2(700, -200);
        [SerializeField] Vector2 _gotoNextStageButtonTargetPosition = new Vector2(800, -350);

        [Header("イージング")]
        [SerializeField] Ease _clearTextEase = Ease.OutBack;
        [SerializeField] Ease _buttonEase = Ease.OutBack;
        [SerializeField] Ease _cameraEase = Ease.Linear;

        private MotionHandle _cameraMotionHandle;
        private MotionHandle _stageClearTextMotionHandle;
        private MotionHandle _gotoStageSelectButtonMotionHandle;
        private MotionHandle _gotoNextStageButtonMotionHandle;
        private Vector2 _stageClearTextInitialPosition;
        private Vector2 _gotoStageSelectButtonInitialPosition;
        private Vector2 _gotoNextStageButtonInitialPosition;

#if UNITY_EDITOR
        void Update()
        {
            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                PlayGameClearAnimation().Forget();
            }
        }
#endif

        void Awake()
        {
            _stageClearTextInitialPosition = _container.StageClearText.rectTransform.anchoredPosition;
            _gotoStageSelectButtonInitialPosition = GetRectTransform(_container.GotoStageSelectButton).anchoredPosition;
            _gotoNextStageButtonInitialPosition = GetRectTransform(_container.NextStageButton).anchoredPosition;
        }

        private static RectTransform GetRectTransform(Component component)
        {
            return component.transform as RectTransform;
        }

        /// <summary>
        /// ステージクリア時のアニメーションを順番に再生する。
        /// </summary>
        /// <returns></returns>
        public async UniTask PlayGameClearAnimation()
        {
            ShowStageClearText(); // ステージクリアのUIをアクティブ化
            _container.ChangeStageClearCamera(); // カメラをステージクリア用のカメラに切り替え
            RotateCamera(); // カメラを回転させるアニメーションを再生
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            PlayStageClearTextAnimation(); // ステージクリアテキストのアニメーションを再生
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            PlayButtonsAnimation(); // ボタンのアニメーションを再生
        }

        private void ShowStageClearText()
        {
            _container.ShowStageClearUI();
        }

        private void RotateCamera()
        {
            if (_container.StageClearCamera == null || _container.OmeletteObject == null)
            {
                return;
            }

            if (_cameraMotionHandle.IsActive())
            {
                _cameraMotionHandle.Cancel();
            }

            Transform cameraTransform = _container.StageClearCamera.transform;

            Transform omeletteTransform = _container.OmeletteObject.transform;

            // 最初のカメラ位置を設定
            Vector3 center = omeletteTransform.position;
            Vector3 startPosition = center + _cameraOrbitOffset;

            cameraTransform.position = startPosition;
            cameraTransform.rotation = Quaternion.LookRotation(center - startPosition, Vector3.up);

            _cameraMotionHandle = LMotion
                .Create(
                    0f,
                    _cameraRotationAngle,
                    _cameraRotationDuration)
                .WithEase(_cameraEase)
                .Bind(angle =>
                {
                    if (cameraTransform == null || omeletteTransform == null)
                    {
                        return;
                    }

                    Vector3 targetPosition = omeletteTransform.position;

                    Quaternion orbitRotation = Quaternion.AngleAxis(angle, Vector3.up);

                    Vector3 rotatedOffset = orbitRotation * _cameraOrbitOffset;

                    Vector3 cameraPosition = targetPosition + rotatedOffset;

                    Quaternion cameraRotation = Quaternion.LookRotation(targetPosition - cameraPosition, Vector3.up);

                    cameraTransform.SetPositionAndRotation(
                        cameraPosition,
                        cameraRotation);
                })
                .AddTo(cameraTransform);
        }

        private void PlayStageClearTextAnimation()
        {
            if (_container.StageClearText == null)
            {
                return;
            }

            if (_stageClearTextMotionHandle.IsActive())
            {
                _stageClearTextMotionHandle.Cancel();
            }

            RectTransform rectTransform = _container.StageClearText.rectTransform;

            _stageClearTextMotionHandle = LMotion
                .Create(
                    rectTransform.anchoredPosition,
                    _stageClearTextTargetPosition,
                    _stageClearTextAnimationDuration)
                .WithEase(_clearTextEase)
                .BindToAnchoredPosition(rectTransform)
                .AddTo(rectTransform);
        }

        private void PlayButtonsAnimation()
        {
            if (_container.GotoStageSelectButton == null || _container.NextStageButton == null)
            {
                return;
            }

            if (_gotoStageSelectButtonMotionHandle.IsActive())
            {
                _gotoStageSelectButtonMotionHandle.Cancel();
            }

            if (_gotoNextStageButtonMotionHandle.IsActive())
            {
                _gotoNextStageButtonMotionHandle.Cancel();
            }

            RectTransform stageSelectRect =
                GetRectTransform(_container.GotoStageSelectButton);

            RectTransform nextStageRect =
                GetRectTransform(_container.NextStageButton);

            // 1つ目：すぐに開始
            _gotoStageSelectButtonMotionHandle = LMotion
                .Create(
                    stageSelectRect.anchoredPosition,
                    _gotoStageSelectButtonTargetPosition,
                    _buttonAnimationDuration)
                .WithEase(_buttonEase)
                .BindToAnchoredPosition(stageSelectRect)
                .AddTo(stageSelectRect);

            // 2つ目：少し遅れて開始
            _gotoNextStageButtonMotionHandle = LMotion
                .Create(
                    nextStageRect.anchoredPosition,
                    _gotoNextStageButtonTargetPosition,
                    _buttonAnimationDuration)
                .WithDelay(_buttonAnimationDelay)
                .WithEase(_buttonEase)
                .BindToAnchoredPosition(nextStageRect)
                .AddTo(nextStageRect);
        }
    }
}