using UnityEngine;
using Alchemy.Inspector;
using TNRD;

namespace Ko.Result
{
    public class ServiceRegisterer : MonoBehaviour
    {
        [SerializeField, LabelText("ステージのコントローラー")] private StageController _stageController;
        [SerializeField, LabelText("ランキングUIのコントローラー")] private RankingUIController _rankingUIController;
        [SerializeField, LabelText("カメラのコントローラー")] private CameraController _cameraController;
        [SerializeField, LabelText("プレイヤーのコントローラー")] private PlayerController _playerController;
        [SerializeField, LabelText("結果発表パネルのコントローラー")] private AnnouncePanelController _announcePanelController;
        [SerializeField, LabelText("プレイヤー位置設定コントローラー")] private SerializableInterface<Key.Casino.PlayerPositionSetter.IController> _playerPositionSetterController;
        [SerializeField, LabelText("リザルトシーンマネージャー")] private ResultManager _resultManager;

        private void Awake()
        {
            RegisterServices();
        }

        private void RegisterServices()
        {
            ServiceLocator.Register<StageController>(_stageController);
            ServiceLocator.Register<RankingUIController>(_rankingUIController);
            ServiceLocator.Register<CameraController>(_cameraController);
            ServiceLocator.Register<PlayerController>(_playerController);
            ServiceLocator.Register<AnnouncePanelController>(_announcePanelController);
            ServiceLocator.Register<Key.Casino.PlayerPositionSetter.IController>(_playerPositionSetterController.Value);
            ServiceLocator.Register<ResultManager>(_resultManager);
        }

        private void UnregisterServices()
        {
            ServiceLocator.Unregister<StageController>();
            ServiceLocator.Unregister<RankingUIController>();
            ServiceLocator.Unregister<CameraController>();
            ServiceLocator.Unregister<PlayerController>();
            ServiceLocator.Unregister<AnnouncePanelController>();
            ServiceLocator.Unregister<Key.Casino.PlayerPositionSetter.IController>();
            ServiceLocator.Unregister<ResultManager>();
        }

        private void OnDestroy()
        {
            UnregisterServices();
        }

    }
}