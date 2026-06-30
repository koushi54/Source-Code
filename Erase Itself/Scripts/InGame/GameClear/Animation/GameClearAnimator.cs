using UnityEngine;
using LitMotion;
using Cysharp.Threading.Tasks;
using System;
using LitMotion.Extensions;

namespace Ko.InGame.GameClear
{
    public class GameClearAnimator : MonoBehaviour
    {
        [SerializeField] private GameClearContainer _container;
        [SerializeField] private float _cameraRotationRadius = 2f;
        private MotionHandle _heavenMotionHandle;


        public async UniTask PlayClearAnimationAsync(float limitTime, float currentTime)
        {
            _container.ShowGameClearUI(limitTime, currentTime);
            _container.GenerateDeadEraser();
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            RotateCamera();
            GoToHeaven().Forget();

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            await PlayClearUIAsync();
        }

        private void RotateCamera()
        {
            // カメラをプレイヤーの周りを公転させるアニメーションを実装
            var playerPos = _container.Player.transform.position;

            LMotion.Create(0f, 360f, 6f)
                .WithLoops(-1)
                .WithEase(Ease.Linear)
                .Bind(angle =>
                {
                    var cameraTransform = _container.ClearCamera.transform;
                    var rotatedOffset = Quaternion.AngleAxis(angle, Vector3.up) * new Vector3(0f, 3f, _cameraRotationRadius);

                    cameraTransform.position = playerPos + rotatedOffset;
                    cameraTransform.rotation = Quaternion.LookRotation(playerPos - cameraTransform.position);
                });
        }

        private async UniTask PlayClearUIAsync()
        {
            // ゲームクリアのテキストを拡大させながら表示するアニメーションを実装
            await LMotion.Create(0f, 1f, 0.5f)
                .WithEase(Ease.OutBack)
                .BindToLocalScaleXYZ(_container.GameClearText.transform);
            // クリアタイムと次のステージへのボタンをフェードインさせるアニメーションを実装
            await LMotion.Create(-1400f, -500f, 0.5f)
                .WithEase(Ease.OutCubic)
                .BindToAnchoredPositionX(_container.ClearTimeText.transform as RectTransform);
            await LMotion.Create(1300f, 750f, 0.5f)
                .WithEase(Ease.OutCubic)
                .BindToAnchoredPositionX(_container.NextButton.transform as RectTransform);
        }

        private async UniTask GoToHeaven()
        {
            var deadEraser = _container.CurrentDeadEraser;
            if (deadEraser == null)
            {
                Debug.LogError("DeadEraserが見つかりませんでした。");
                return;
            }

            var startPos = deadEraser.transform.position;

            // 以前のハンドルがもし動いていれば安全にキャンセル
            if (_heavenMotionHandle.IsActive())
            {
                _heavenMotionHandle.Cancel();
            }

            // 各種設定値
            float duration = 10f;       // 上昇にかかる時間（秒）
            float waveSpeed = 20f;     // 横揺れの速さ
            float waveAmplitude = 1.5f;  // 横揺れの幅

            // 0 から 1 まで、5秒かけて変化する時間軸のモーションを1つだけ作成
            _heavenMotionHandle = LMotion.Create(0f, 1f, duration)
                .WithEase(Ease.Linear)
                .Bind(t =>
                {
                    if (deadEraser == null) return;

                    float yOffset = Mathf.Lerp(0f, 15f, t);

                    float horizontalOffset = Mathf.Sin(t * waveSpeed) * waveAmplitude;

                    deadEraser.transform.position = startPos + Vector3.up * yOffset + Vector3.right * horizontalOffset;
                });

            // 5秒間のモーションが終わるのを待つ
            await _heavenMotionHandle.ToUniTask(this.destroyCancellationToken);

            // 終了後にオブジェクトを破棄
            if (deadEraser != null)
            {
                Destroy(deadEraser);
            }
        }
    }
}
