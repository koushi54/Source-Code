using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace Ko.StageClear
{
    public class StageClearContainer : MonoBehaviour
    {
        [Header("カメラ")]
        [SerializeField] CinemachineCamera _mainCamera;
        [SerializeField] CinemachineCamera _stageClearCamera;
        [Header("UI")]
        [SerializeField] TMP_Text _stageClearText;
        [SerializeField] Button _nextStageButton;
        [SerializeField] Button _gotoStageSelectButton;

        [Header("オブジェクト")]
        [SerializeField] GameObject _panObject;
        [SerializeField] GameObject _omeletteObject;

        public CinemachineCamera MainCamera => _mainCamera;
        public CinemachineCamera StageClearCamera => _stageClearCamera;
        public TMP_Text StageClearText => _stageClearText;
        public Button NextStageButton => _nextStageButton;
        public Button GotoStageSelectButton => _gotoStageSelectButton;
        public GameObject PanObject => _panObject;
        public GameObject OmeletteObject => _omeletteObject;

        void Awake()
        {
            HideStageClearUI();
        }

        public void ChangeStageClearCamera()
        {
            _mainCamera.Priority = Key.Core.CameraParameter.DISABLE_CAMERA_PARAMETER;
            _stageClearCamera.Priority = Key.Core.CameraParameter.ENABLE_CAMERA_PARAMETER;
        }

        public void ShowStageClearUI()
        {
            _stageClearText.gameObject.SetActive(true);
            _nextStageButton.gameObject.SetActive(true);
            _gotoStageSelectButton.gameObject.SetActive(true);
        }

        public void HideStageClearUI()
        {
            _stageClearText.gameObject.SetActive(false);
            _nextStageButton.gameObject.SetActive(false);
            _gotoStageSelectButton.gameObject.SetActive(false);
        }
    }
}

