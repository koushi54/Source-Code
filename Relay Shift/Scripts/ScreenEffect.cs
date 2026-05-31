using UnityEngine;
using UnityEngine.UI;
using LitMotion;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;

namespace Ko
{
    public class ScreenEffect : MonoBehaviour
    {
        private static ScreenEffect _instance;
        public static ScreenEffect Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<ScreenEffect>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("ScreenEffect");
                        _instance = go.AddComponent<ScreenEffect>();
                    }
                }
                return _instance;
            }
        }
        [Header("エフェクト用のUIパネル")]
        [SerializeField] private Image _fadePanel;
        [SerializeField] private Image _flashPanel;
        private MotionHandle _currentMotion;
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            // 初期状態は透明にしておく
            _fadePanel.color = new Color(0, 0, 0, 0);
            _flashPanel.color = new Color(1, 1, 1, 0);
        }

        public async UniTask FadeIn(float duration)
        {
            CancelCurrentEffect();
            _currentMotion = LMotion.Create(1f, 0f, duration)
                .WithEase(Ease.OutQuad)
                .Bind(_fadePanel, (val, target) =>
                {
                    Color c = target.color;
                    c.a = val;
                    target.color = c;
                });
            await _currentMotion.ToAwaitable();
        }

        public async UniTask FadeOut(float duration)
        {
            CancelCurrentEffect();
            _currentMotion = LMotion.Create(0f, 1f, duration)
                .WithEase(Ease.OutQuad)
                .Bind(_fadePanel, (val, target) =>
                {
                    Color c = target.color;
                    c.a = val;
                    target.color = c;
                });
            await _currentMotion.ToAwaitable();
        }

        public async UniTask Flash(float duration)
        {
            CancelCurrentEffect();
            _currentMotion = LMotion.Create(0f, 1f, duration / 2)
                .WithEase(Ease.OutQuad)
                .WithLoops(2, LoopType.Yoyo)
                .Bind(_flashPanel, (val, target) =>
                {
                    Color c = target.color;
                    c.a = val;
                    target.color = c;
                });
            await _currentMotion.ToAwaitable();
        }

        private void CancelCurrentEffect()
        {
            if (_currentMotion.IsActive())
            {
                _currentMotion.Cancel();
            }
            _fadePanel.color = new Color(0, 0, 0, 0);
            _flashPanel.color = new Color(1, 1, 1, 0);
        }

        private void OnDestroy()
        {
            if (_currentMotion.IsActive())
            {
                _currentMotion.Cancel();
            }
        }
    }
}