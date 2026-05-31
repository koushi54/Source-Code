using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using Cysharp.Threading.Tasks;
using Alchemy.Inspector;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Ko.Result
{
    public class CameraController : NetworkBehaviour
    {
        [Title("カメラ")]
        [SerializeField] private CinemachineCamera _defaultCam;
        [SerializeField] private CinemachineCamera _firstCam;
        [SerializeField] private CinemachineCamera _secondCam;
        [SerializeField] private CinemachineCamera _thirdCam;
        [SerializeField] private CinemachineCamera _camera1;
        [SerializeField] private CinemachineCamera _camera2;
        [Title("フェード用")]
        [SerializeField] private Image _fadeImage;
        [Title("その他")]
        [SerializeField] private Volume _volume;
        private Vector3 _cam1Pos;
        private Vector3 _cam2Pos;
        private Sequence _preAnimationSequence;
        private const int ACTIVE_PRIORITY = 20;
        private const int INACTIVE_PRIORITY = 10;

        // カメラの初期設定
        [Rpc(SendTo.Everyone)]
        public void InitializeCameraRpc(NetworkObjectReference[] topPlayerObjects)
        {
            //カメラの初期位置を保存
            _cam1Pos = _camera1.transform.position;
            _cam2Pos = _camera2.transform.position;

            // カメラの初期設定
            _defaultCam.Priority = ACTIVE_PRIORITY;
            _firstCam.Priority = INACTIVE_PRIORITY;
            _secondCam.Priority = INACTIVE_PRIORITY;
            _thirdCam.Priority = INACTIVE_PRIORITY;
            _camera1.Priority = INACTIVE_PRIORITY;
            _camera2.Priority = INACTIVE_PRIORITY;

            for (int i = 0; i < topPlayerObjects.Length; i++)
            {
                var playerObj = topPlayerObjects[i].TryGet(out NetworkObject obj) ? obj : null;
                switch (i)
                {
                    case 0:
                        _firstCam.Follow = playerObj.transform;
                        break;
                    case 1:
                        _secondCam.Follow = playerObj.transform;
                        break;
                    case 2:
                        _thirdCam.Follow = playerObj.transform;
                        break;
                    default:
                        break;
                }
            }

            _fadeImage.gameObject.SetActive(false);
        }

        // カメラ切り替え
        [Rpc(SendTo.Everyone)]
        public void SwitchCameraRpc(int camNum)
        {
            switch (camNum)
            {
                // camNum: 0=デフォルト、1=1位プレイヤー、2=2位プレイヤー、3=3位プレイヤー、4=カメラ1、5=カメラ2
                case 0:
                    _defaultCam.Priority = ACTIVE_PRIORITY;
                    _firstCam.Priority = INACTIVE_PRIORITY;
                    _secondCam.Priority = INACTIVE_PRIORITY;
                    _thirdCam.Priority = INACTIVE_PRIORITY;
                    _camera1.Priority = INACTIVE_PRIORITY;
                    _camera2.Priority = INACTIVE_PRIORITY;
                    break;
                case 1:
                    _defaultCam.Priority = INACTIVE_PRIORITY;
                    _firstCam.Priority = ACTIVE_PRIORITY;
                    _secondCam.Priority = INACTIVE_PRIORITY;
                    _thirdCam.Priority = INACTIVE_PRIORITY;
                    _camera1.Priority = INACTIVE_PRIORITY;
                    _camera2.Priority = INACTIVE_PRIORITY;
                    break;
                case 2:
                    _defaultCam.Priority = INACTIVE_PRIORITY;
                    _firstCam.Priority = INACTIVE_PRIORITY;
                    _secondCam.Priority = ACTIVE_PRIORITY;
                    _thirdCam.Priority = INACTIVE_PRIORITY;
                    _camera1.Priority = INACTIVE_PRIORITY;
                    _camera2.Priority = INACTIVE_PRIORITY;
                    break;
                case 3:
                    _defaultCam.Priority = INACTIVE_PRIORITY;
                    _firstCam.Priority = INACTIVE_PRIORITY;
                    _secondCam.Priority = INACTIVE_PRIORITY;
                    _thirdCam.Priority = ACTIVE_PRIORITY;
                    _camera1.Priority = INACTIVE_PRIORITY;
                    _camera2.Priority = INACTIVE_PRIORITY;
                    break;
                case 4:
                    _defaultCam.Priority = INACTIVE_PRIORITY;
                    _firstCam.Priority = INACTIVE_PRIORITY;
                    _secondCam.Priority = INACTIVE_PRIORITY;
                    _thirdCam.Priority = INACTIVE_PRIORITY;
                    _camera1.Priority = ACTIVE_PRIORITY;
                    _camera2.Priority = INACTIVE_PRIORITY;
                    break;
                case 5:
                    _defaultCam.Priority = INACTIVE_PRIORITY;
                    _firstCam.Priority = INACTIVE_PRIORITY;
                    _secondCam.Priority = INACTIVE_PRIORITY;
                    _thirdCam.Priority = INACTIVE_PRIORITY;
                    _camera1.Priority = INACTIVE_PRIORITY;
                    _camera2.Priority = ACTIVE_PRIORITY;
                    break;
                default:
                    break;
            }
            
        }


        /// <summary>
        /// カメラをデフォルトにリセットし、各カメラの位置も初期位置に戻す
        /// </summary>
        public void ResetCamera()
        {
            _defaultCam.Priority = ACTIVE_PRIORITY;
            _firstCam.Priority = INACTIVE_PRIORITY;
            _secondCam.Priority = INACTIVE_PRIORITY;
            _thirdCam.Priority = INACTIVE_PRIORITY;
            _camera1.Priority = INACTIVE_PRIORITY;
            _camera2.Priority = INACTIVE_PRIORITY;
        }

        public void ResetCameraPosition()
        {
            _camera1.transform.position = _cam1Pos;
            _camera2.transform.position = _cam2Pos;
        }

        /// <summary>
        /// カメラフェードアニメーション
        /// </summary>
        public void FadeOutCamera(float duration, int camNum)
        {
            // フェードアウト
            _fadeImage.gameObject.SetActive(true);
            _fadeImage.color = new Color(0f, 0f, 0f, 0f); // 初期透明度を0に設定
            _fadeImage.DOFade(1f, duration).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                // フェードアウト完了後の処理（必要に応じて追加）
                SwitchCameraRpc(camNum); // カメラ切り替えを開始
                FadeInCamera(duration); // フェードインを開始
            });
        }

        [Rpc(SendTo.Everyone)]
        public void FadeOutCameraRpc(float duration, int camNum)
        {
            FadeOutCamera(duration, camNum);
        }

        public void FadeInCamera(float duration)
        {
            // フェードイン
            _fadeImage.DOFade(0f, duration).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                // フェードイン完了後の処理（必要に応じて追加）
                _fadeImage.gameObject.SetActive(false); // フェードイメージを非表示にする
            });
        }

        /// <summary>
        /// 1位のアニメーション前にカメラ(2nd,3rd)を移動させる
        /// </summary>
        public void PreAnimationCameraMoving()
        {
            const float thirdMoveDuration = 3f;
            const float secondMoveDuration = 3f;

            ResetCameraPositionRpc();
            _camera1.transform.DOKill();
            _camera2.transform.DOKill();
            _fadeImage.DOKill();
            _preAnimationSequence?.Kill();

            _preAnimationSequence = DOTween.Sequence();
            _preAnimationSequence.Append(_camera2.transform.DOMove(new Vector3(_cam2Pos.x + 1.5f, _cam2Pos.y, _cam2Pos.z + 1.5f), thirdMoveDuration).SetEase(Ease.Linear))
                .Join(ChangeVignetteIntensity(0.65f, 0.5f))
                .InsertCallback(0.5f,() =>
                {
                    if (IsServer)
                    {
                        FadeOutCameraRpc(0.3f, 5);
                    }
                })
                .Append(_camera1.transform.DOMove(new Vector3(_cam1Pos.x - 1.5f, _cam1Pos.y, _cam1Pos.z - 1.5f), secondMoveDuration).SetEase(Ease.Linear))
                .InsertCallback(3.25f, () =>
                {
                    // 2ndカメラ移動中に既存のフェード付き切り替え処理を実行
                    if (IsServer)
                    {
                        FadeOutCameraRpc(0.3f, 4);
                    }
                })
                .OnComplete(() =>
                {
                    ChangeVignetteIntensity(0f, 0.5f);
                    // カメラ移動完了後の処理（必要に応じて追加）
                    _fadeImage.gameObject.SetActive(false);
                });
                
        }

        private Tween ChangeVignetteIntensity(float targetIntensity, float duration)
        {
            if (_volume == null || _volume.profile == null)
            {
                return DOTween.Sequence();
            }

            if (!_volume.profile.TryGet<Vignette>(out var vignette))
            {
                return DOTween.Sequence();
            }

            var clampedIntensity = Mathf.Clamp01(targetIntensity);
            return DOTween.To(() => vignette.intensity.value,
                value => vignette.intensity.value = value,
                clampedIntensity,
                duration)
                .SetEase(Ease.Linear);
        }

        [Rpc(SendTo.Everyone)]
        public void PreAnimationCameraMovingRpc()
        {
            PreAnimationCameraMoving();
        }

        [Rpc(SendTo.Everyone)]
        public void ResetCameraRpc()
        {
            ResetCamera();
        }

        [Rpc(SendTo.Everyone)]
        public void ResetCameraPositionRpc()
        {
            ResetCameraPosition();
        }
    }
}