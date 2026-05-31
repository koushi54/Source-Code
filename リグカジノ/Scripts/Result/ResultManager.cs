using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alchemy.Inspector;
using AudioManager.SE;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ko.Result
{
    public class ResultManager : NetworkBehaviour
    {
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private StageController _stageController;
        [SerializeField] private RankingUIController _rankingUIController;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private ClosingTextController _closingTextController;
        [SerializeField] private EffectManager _effectManager;
        [Title("ステージの高さ")]
        [SerializeField] private float[] _stageHeights;
        [SerializeField] private float _initialStageHeight = 1f;
        [SerializeField] private float _playerRiseAdditionalOffset = 0.45f;
        [Title("演出タイミング")]
        [SerializeField] private float _riseDuration = 1f;
        [SerializeField] private float _uiShowDelayAfterRise = 0.1f;
        [SerializeField] private float _cameraFadeDuration = 0.3f;
        [SerializeField] private float _intervalAfterCamera = 1.7f;
        private float[] _currentStageHeights;

        #if UNITY_EDITOR
        private void Update()
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                //ShowResultAnimation(0);
            }
            if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                //ShowResultAnimation(1);
            }
            if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                //ShowResultAnimation(2);
            }
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                //ShowAllResultAnimations(topPlayerIds, topPlayerObjects).Forget();
            }
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                _cameraController.ResetCameraPosition();
            }
        }
        #endif

        // ランクに応じたアニメーションを再生する処理
        private void ShowResultAnimation(int rank, int[] topPlayerIds, NetworkObjectReference[] topPlayerObjects)
        {
            int rankIndex = rank - 1;
            float targetStageHeight = _stageHeights[rankIndex];
            float previousStageHeight = _currentStageHeights[rankIndex];
            float stageHeightDelta = targetStageHeight - previousStageHeight;
            float playerHeightDelta = stageHeightDelta + _playerRiseAdditionalOffset;

            _stageController.ChangeStageHeightRpc(rankIndex, targetStageHeight, topPlayerIds, _riseDuration); // ステージの高さを変更
            _playerController.MovingPlayerToTopPositionRpc(rankIndex, playerHeightDelta, topPlayerObjects, _riseDuration); // プレイヤーを上昇量だけ移動させる

            _currentStageHeights[rankIndex] = targetStageHeight;
        }

        // 1つのランクアニメーションを順番に再生する処理
        private async UniTask PlayRankAnimationAsync(int rank, int[] topPlayerIds, NetworkObjectReference[] topPlayerObjects)
        {
            if (rank < 1 || rank > topPlayerIds.Length) return;
            _cameraController.FadeOutCameraRpc(_cameraFadeDuration, rank);

            ShowResultAnimation(rank, topPlayerIds, topPlayerObjects);

            await UniTask.Delay((int)(_riseDuration * 1000f));

            if (rank == 1) SEManager.Instance.Play(SEName.Celebrate);
            else SEManager.Instance.Play(SEName.Clap);
            PlayPlayerAnimation(rank, topPlayerObjects[rank-1]);

            await UniTask.Delay((int)(_uiShowDelayAfterRise * 1000f));
            _rankingUIController.ShowRankingUIRpc(rank);

            await UniTask.Delay(TimeSpan.FromSeconds(3f)); // ランクアニメーションとUI表示を見せる時間
        }

        // 全てのランクアニメーションを順番に再生する処理
        public async UniTask ShowAllResultAnimationsAsync(int[] topPlayerIds, NetworkObjectReference[] topPlayerObjects)
        {
            _currentStageHeights = new float[_stageHeights.Length];
            for (int i = 0; i < _currentStageHeights.Length; i++)
            {
                _currentStageHeights[i] = _initialStageHeight;
            }

            _rankingUIController.HideAllRankingUIRpc();

            await PlayRankAnimationAsync(3, topPlayerIds, topPlayerObjects);
            await PlayRankAnimationAsync(2, topPlayerIds, topPlayerObjects);

            _cameraController.ResetCameraRpc();
            DOVirtual.DelayedCall(0.5f, () => SEManager.Instance.Play(SEName.TimpaniRoll));
            _cameraController.PreAnimationCameraMovingRpc(); // 最後のアニメーション前にカメラを移動
            await UniTask.Delay(TimeSpan.FromSeconds(6.5f));
            // SEManager.Instance.Stop(SEName.TimpaniRoll);

            await PlayRankAnimationAsync(1, topPlayerIds, topPlayerObjects);

            _cameraController.ResetCameraRpc();
            _cameraController.ResetCameraPositionRpc();
            _closingTextController.ShowClosingTextRpc();

            // 入賞できなかったプレイヤーの敗北アニメーションを再生
            foreach (var playerObj in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (!topPlayerObjects.Contains(playerObj.PlayerObject))
                {
                    PlayPlayerAnimation(0, playerObj.PlayerObject);
                }
            }

            // 花火エフェクトを再生
            _effectManager.PlayFireworksRpc();

            while (true)
            {
                _rankingUIController.AnimateAllRankingUIsRpc();
                await UniTask.Delay(TimeSpan.FromSeconds(3f)); // アニメーションの合計時間に合わせて待機
            }
        }

        private void PlayPlayerAnimation(int rank, NetworkObjectReference playerObject)
        {
            playerObject.TryGet(out NetworkObject obj);
            if (obj != null)
            {
                Key.Player.PlayerAnimationController animationController = obj.GetComponent<Key.Player.PlayerAnimationController>();
                if (animationController != null)
                {
                    switch (rank)
                    {
                        case 1:
                            animationController.PlayResultAnimationRpc(2); // 優勝アニメーション
                            break;
                        case 2:
                            animationController.PlayResultAnimationRpc(1); // 入賞アニメーション
                            break;
                        case 3:
                            animationController.PlayResultAnimationRpc(1); // 入賞アニメーション
                            break;
                        default:
                            animationController.PlayResultAnimationRpc(0); // 負けアニメーション
                            break;
                    }
                }
            }
        }

    }
}
