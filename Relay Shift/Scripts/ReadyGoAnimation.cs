using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine.InputSystem;
using TMPro;

namespace Ko
{
    public class ReadyGoAnimation : MonoBehaviour
    {
        [Header("ReadyGoのUI")]
        [SerializeField] private RectTransform _rollingText;
        [SerializeField] private CanvasGroup _rollingCanvasGroup; 
        [SerializeField] private TMP_Text _goText;
        private float _originalRotationX;
        private MotionHandle _rollingMotion;
        private MotionHandle _hideMotion;
        
        #if UNITY_EDITOR
        private void Update()
        {
            if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                PlayReadyGoAnimation().Forget();
            }
        }
        #endif
        private void Start()
        {
            _rollingText.gameObject.SetActive(false);
            _originalRotationX = _rollingText.transform.rotation.eulerAngles.x;
        }

        public async UniTask PlayReadyGoAnimation()
        {
            try
            {
                if (_rollingMotion.IsActive())
                {
                    _rollingMotion.Cancel();
                }
            }
            catch {}

            _rollingText.gameObject.SetActive(true);

            // (よーい) — Quaternion を使って補間（Euler のラップを回避）
            Quaternion startQuat = Quaternion.Euler(_originalRotationX, _rollingText.localEulerAngles.y, _rollingText.localEulerAngles.z);
            Quaternion midQuat = Quaternion.Euler(_originalRotationX - 90f, _rollingText.localEulerAngles.y, _rollingText.localEulerAngles.z);
            _rollingMotion = LMotion.Create(0f, 1f, 0.5f)
                .WithEase(Ease.OutQuad)
                .Bind(_rollingText, (t, target) =>
                {
                    target.localRotation = Quaternion.Slerp(startQuat, midQuat, t);
                });

            await _rollingMotion.ToUniTask();
            await UniTask.Delay(System.TimeSpan.FromSeconds(2f));

            // (どん) — mid -> end を Quaternion で補間
            Quaternion endQuat = Quaternion.Euler(_originalRotationX - 180f, _rollingText.localEulerAngles.y, _rollingText.localEulerAngles.z);
            _rollingMotion = LMotion.Create(0f, 1f, 0.5f)
                .WithEase(Ease.OutQuad)
                .Bind(_rollingText, (t, target) =>
                {
                    target.localRotation = Quaternion.Slerp(midQuat, endQuat, t);
                });

            await _rollingMotion.ToUniTask();

            HideReadyGoAnimation();
        }

        public void HideReadyGoAnimation()
        {
            if (_hideMotion.IsActive())
            {
                _hideMotion.Cancel();
            }
            // 文字間隔が広がりながらフェードアウトするアニメーション
            _hideMotion = LSequence.Create()
                .Append(LMotion.Create(0f, 30f, 1f)
                    .WithEase(Ease.OutQuad)
                    .Bind(_goText, (val, target) =>
                    {
                        target.characterSpacing = val;
                    }))
                .Join(LMotion.Create(1f, 0f, 1f)
                    .WithEase(Ease.OutQuad)
                    .Bind(_rollingCanvasGroup, (val, target) =>
                    {
                        target.alpha = val;
                    })).Run();
        }
    }
}