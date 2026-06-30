using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Ko.InGame.GameClear
{
    public class MockGameClearService : MonoBehaviour, IGameClearService
    {
        public async UniTask PlayGameClearAnimationAsync(float limitTime, float currentTime)
        {
            Debug.Log("Mock: アニメーション再生開始");
            await UniTask.Delay(1000); // 模擬的な遅延
            Debug.Log("Mock: アニメーション再生終了");
        }

        public void GoToNextStage()
        {
            Debug.Log("Mock: 次のステージへ…");
            // 模擬的な次のステージへの遷移処理
        }
    }
}