using UnityEngine;
using LitMotion;
using Cysharp.Threading.Tasks;
using System;
using LitMotion.Extensions;
using Alchemy.Inspector;

namespace Ko.InGame.GameOver
{
    public class GameOverAnimator : MonoBehaviour
    {
        [SerializeField] private GameOverContainer _gameOverContainer;

        private MotionHandle _heavenMotionHandle;

        public async UniTask PlayGameOverAnimation()
        {

            ChangeBrightness(_gameOverContainer.DirectionalLight.intensity, 0.2f);
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            await PlayUIAnimation();
        }

        private async UniTask GoToHeaven()
        {
            var deadEraser = _gameOverContainer.CurrentDeadEraser;
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

        // private void PlayWingAnimation()
        // {
        //     var wing = _gameOverContainer.Wing;
        //     if (wing == null)
        //     {
        //         Debug.LogError("Wingが見つかりませんでした。");
        //         return;
        //     }

        //     // 以前のハンドルがもし動いていれば安全にキャンセル
        //     if (_wingMotionHandle.IsActive())
        //     {
        //         _wingMotionHandle.Cancel();
        //     }

        //     _wingMotionHandle = LMotion.Create(0f, Math.PI * 2f, 0.8f)
        //         .WithEase(Ease.Linear)
        //         .WithLoops(-1)
        //         .Bind(t =>
        //         {
        //             float angleX = Mathf.Sin((float)t) * 10f; // 30度の範囲で前後に揺れる
        //             wing.transform.localRotation = Quaternion.Euler(angleX, 0f, 0f);
        //         });
        // }

        private async UniTask PlayUIAnimation()
        {
            var gameOverText = _gameOverContainer.GameOverText;
            var continueButton = _gameOverContainer.ContinueButton;

            if (gameOverText == null || continueButton == null)
            {
                Debug.LogError("GameOverTextまたはContinueButtonが見つかりませんでした。");
                return;
            }
            _gameOverContainer.ShowGameOverUI();
            await LMotion.Create(_gameOverContainer.GameOverTextInitialPos.y, 0f, 2f)
                .WithEase(Ease.OutBounce)
                .BindToAnchoredPositionY(gameOverText.GetComponent<RectTransform>());
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f)); // テキストが少し落ち着くのを待つ
            await LMotion.Create(_gameOverContainer.ContinueButtonInitialPos.x, 850f, 1.5f)
                .WithEase(Ease.OutBack)
                .BindToAnchoredPositionX(continueButton.GetComponent<RectTransform>());
        }

        public void ChangeBrightness(float currentIntensity, float targetIntensity)
        {
            LMotion.Create(currentIntensity, targetIntensity, 0.5f)
                .WithEase(Ease.Linear)
                .Bind(t =>
                {
                    if (_gameOverContainer.DirectionalLight == null) return;

                    _gameOverContainer.DirectionalLight.intensity = t;
                });
        }
    }
}