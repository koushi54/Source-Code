using UnityEngine;
using Cysharp.Threading.Tasks;
using Kakky;
using R3;
using Alchemy.Inspector;
using KanKikuchi.AudioManager;

namespace Ko.InGame.GameOver
{
    public class GameOverService : MonoBehaviour, IGameOverService
    {
        [SerializeField] private GameDataContext _gameDataContext;
        [SerializeField] private GameOverContainer _gameOverContainer;
        [SerializeField] private GameOverAnimator _gameOverAnimator;
        [SerializeField] private GameObject _inGameUI;

        private float _intialStrength;
        private float _intialRemainAmount;

        [LabelText("ゲームオーバーアニメーション再生"), Button]
        public async UniTask PlayGameOverAnimationAsync()
        {
            BGMManager.Instance.Stop();
            SEManager.Instance.Play(SEPath.EXPLOSION);
            // ゲームオーバーアニメーションの再生処理を実装
            _gameOverAnimator.PlayGameOverAnimation().Forget();
            HideInGameUI();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SEManager.Instance.Play(SEPath.GAMEOVER);
        }

        public void ContinueGame()
        {
            // ゲームを続ける処理を実装
            // タイマー、プレイヤーのパラメータ等をリセット
            ResetData();
            _gameOverContainer.HideGameOverUI();
            _gameOverAnimator.ChangeBrightness(_gameOverContainer.DirectionalLight.intensity, 1f);
        }

        private void HideInGameUI()
        {
            _inGameUI.SetActive(false);
        }

        private void ResetData()
        {
            _gameDataContext.PlayerParamData.Strength.Value = _intialStrength;
            _gameDataContext.PlayerParamData.RemainAmount.Value = _intialRemainAmount;
            _gameDataContext.TimerData.CurrentTime.Value = _gameDataContext.StageData.LimitTime.Value;
        }

        void Start()
        {
            _intialRemainAmount = _gameDataContext.PlayerParamData.RemainAmount.Value;
            _intialStrength = _gameDataContext.PlayerParamData.Strength.Value;
            _gameOverContainer.ContinueButton.OnClickAsObservable()
            .Subscribe(_ => ContinueGame())
            .AddTo(this);
        }
    }


}
