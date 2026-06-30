using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Ko.InGame.GameOver
{
    public class MockGameOverService : MonoBehaviour, IGameOverService
    {
        public async UniTask PlayGameOverAnimationAsync()
        {
            // モックのゲームオーバーアニメーションの再生処理を実装
            Debug.Log("Mock: アニメーション再生開始");
            await UniTask.Delay(2000); // 2秒待機してアニメーションが再生されたと仮定
            Debug.Log("Mock: アニメーション再生終了");
        }

        public void ContinueGame()
        {
            // モックのゲームを続ける処理を実装
            // プレイヤーのHPを回復させる、または最後のチェックポイントから再開するなどの処理を実装
            Debug.Log("Mock: コンティニュー処理実行");
        }
    }
}