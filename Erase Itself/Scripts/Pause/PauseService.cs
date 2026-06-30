using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace Ko.InGame.Pause
{
    public class PauseService : MonoBehaviour, IPauseService
    {
        [SerializeField] private GameObject _settingPanel;

        private bool _isShowCursor;
        private CursorLockMode _currentCursorLockMode;

        public static bool IsPaused { get; private set; }

        void Start()
        {
            _settingPanel.gameObject.SetActive(false);
        }
        public void ShowPauseMenu()
        {
            _isShowCursor = Cursor.visible;
            _currentCursorLockMode = Cursor.lockState;
            // ポーズメニューを表示する処理を実装
            _settingPanel.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            IsPaused = true;
            Time.timeScale = 0f; // ゲームを一時停止する
        }

        public void HidePauseMenu()
        {
            // ポーズメニューを非表示にする処理を実装
            Cursor.lockState = _currentCursorLockMode;
            Cursor.visible = _isShowCursor;
            IsPaused = false;
            _settingPanel.gameObject.SetActive(false);
            Time.timeScale = 1f; // ゲームを再開する
        }

        void Update()
        {
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                ShowPauseMenu();
            }
        }
    }
}