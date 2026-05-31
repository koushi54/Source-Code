using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitMotion;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

namespace Ko
{
    public class HowToPlayPanelController : MonoBehaviour
    {
        [SerializeField] private Image _howToPlayImage;
        [SerializeField] private Vector2 _panelShownPosition = new Vector2(0, -50);
        private MotionHandle _panelMotion;
        private Vector2 _panelOriginalPosition;
        public bool IsShowing = false;


        private void Start()
        {
            _howToPlayImage.gameObject.SetActive(false);
            _panelOriginalPosition = _howToPlayImage.rectTransform.anchoredPosition;
        }

        private void Update()
        {
            if (IsShowing && Keyboard.current.backspaceKey.wasPressedThisFrame)
            {
                HideHowToPlayPanel().Forget();
            }
        }

        public void ShowHowToPlayPanel()
        {
            if (!IsShowing)
            {
                IsShowing = true;
                _howToPlayImage.gameObject.SetActive(true);
                _panelMotion = LMotion.Create(_panelOriginalPosition, _panelShownPosition, 1f)
                    .WithEase(Ease.OutBack)
                    .Bind(_howToPlayImage.rectTransform, (val, target) =>
                    {
                        target.anchoredPosition = val;
                    });
            }
        }

        public async UniTask HideHowToPlayPanel()
        {
            if (IsShowing)
            {
                IsShowing = false;

                _panelMotion = LMotion.Create(_panelShownPosition, _panelOriginalPosition, 1f)
                    .WithEase(Ease.InBack)
                    .Bind(_howToPlayImage.rectTransform, (val, target) =>
                    {
                        target.anchoredPosition = val;
                    });
                await _panelMotion.ToAwaitable();
                _howToPlayImage.gameObject.SetActive(false);
            }
        }
    }
}