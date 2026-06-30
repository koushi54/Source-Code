using UnityEngine;
using Cysharp.Threading.Tasks;
using Kakky;
using Alchemy.Inspector;
using R3;
using KanKikuchi.AudioManager;

namespace Ko.InGame.GameClear
{
    public class GameClearService : MonoBehaviour, IGameClearService
    {
        [SerializeField] private GameDataContext _gameDataContext;
        [SerializeField] private GameClearContainer _gameClearContainer;
        [SerializeField] private GameClearAnimator _gameClearAnimator;
        [SerializeField] private GameObject _inGameUI;

        [LabelText("ゲームクリアアニメーション"), Button]
        public async UniTask PlayGameClearAnimationAsync(float limitTime, float currentTime)
        {
            _inGameUI.SetActive(false);
            BGMManager.Instance.Stop();
            SEManager.Instance.Play(SEPath.GAMECLEAR);
            // ゲームクリアアニメーションの再生処理を実装
            await _gameClearAnimator.PlayClearAnimationAsync(limitTime, currentTime);
        }

        public void GoToNextStage()
        {
            // 次のステージへの移動処理を実装
            // SOのStageDatabaseのCurrentStageIndexをインクリメントして、次のステージに遷移する処理を実装
            _gameDataContext.StageDataBase.CurrentStageIndex.Value++;
            // 演出、UIを消去
            _gameClearContainer.HideGameClearUI();
        }

        void Start()
        {
            _gameClearContainer.NextButton.OnClickAsObservable()
            .Subscribe(_ => GoToNextStage())
            .AddTo(this);
        }
    }
}
