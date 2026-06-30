using UnityEngine;
using UnityEngine.EventSystems;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine.UI;

namespace Ko.Pause
{
    public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private ButtonSEManager _buttonSEManager;
        private RectTransform _rect;
        private Button _button;
        private Vector2 _initialPosition;
        private MotionHandle _motionHandle;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _button = GetComponent<Button>();
            _initialPosition = _rect.anchoredPosition;
        }

        private void OnEnable()
        {
            if (_motionHandle.IsPlaying())
            {
                _motionHandle.Cancel();
            }

            _rect.anchoredPosition = _initialPosition;
        }

        private void OnEnterPointer()
        {
            if (_motionHandle.IsPlaying())
            {
                _motionHandle.Cancel();
            }

            _motionHandle = LMotion.Create(
                    _initialPosition,
                    _initialPosition + new Vector2(-70, 0),
                    0.1f)
                .WithEase(Ease.OutCubic)
                .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
                .BindToAnchoredPosition(_rect);
            _buttonSEManager.PlayHoveringButtonSE();
        }

        private void OnExitPointer()
        {
            if (_motionHandle.IsPlaying())
            {
                _motionHandle.Cancel();
            }

            _motionHandle = LMotion.Create(
                    _initialPosition + new Vector2(-70, 0),
                    _initialPosition,
                    0.1f)
                .WithEase(Ease.OutCubic)
                .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
                .BindToAnchoredPosition(_rect);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnEnterPointer();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnExitPointer();
        }
    }
}