using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;

namespace Ko
{
    public class OptionController : MonoBehaviour
    {
        [Header("Options")]
        [SerializeField] private TMP_Text[] _options;
        [SerializeField] private TitleAnimation _titleAnimation;
        [SerializeField] private HowToPlayPanelController _howToPlayPanelController;
        [SerializeField] private SettingPanelStateMachine _settingPanelStateMachine;
        private int _normalfontSize = 60;
        private int _selectedFontSize = 80;
        private int _currentSelection = 0;
        private bool _isGameStarting = false; // 連打制限用フラグ

        private void Start()
        {

            if (_options == null || _options.Length == 0)
            {
                LoggerManager.LogWarning("OptionController: _options is not assigned or empty.");
                return;
            }
            UpdateOptions(_currentSelection);
        }

        private void Update()
        {
            if (_options == null || _options.Length == 0)
            {
                return;
            }
            if ((_howToPlayPanelController != null && _howToPlayPanelController.IsShowing) ||
                (_settingPanelStateMachine != null && _settingPanelStateMachine.IsShowing))
            {
                return;
            }
            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                // Move selection up
                _currentSelection--;
                if (_currentSelection < 0)
                    _currentSelection = _options.Length - 1;
                UpdateOptions(_currentSelection);
            }
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                // Move selection down
                _currentSelection++;
                if (_currentSelection >= _options.Length)
                    _currentSelection = 0;
                UpdateOptions(_currentSelection);
            }
            else if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                // Select option
                SelectOption(_currentSelection);
            }
        }

        private void UpdateOptions(int index)
        {
            for (int i = 0; i < _options.Length; i++)
            {
                if (i == index)
                {
                    _options[i].fontSize = _selectedFontSize;
                }
                else
                {
                    _options[i].fontSize = _normalfontSize;
                }
            }
        }

        private async void SelectOption(int index)
        {
            if (_isGameStarting)
            {
                LoggerManager.Log("SelectOption: Game is already starting, ignoring input.");
                return;
            }
            // Handle option selection（安全チェック）
            if (_options == null || index < 0 || index >= _options.Length)
            {
                LoggerManager.LogWarning($"SelectOption: invalid index {index} or _options not set.");
                return;
            }
            if (_titleAnimation == null)
            {
                LoggerManager.LogWarning("SelectOption: TitleAnimation reference is null.");
                return;
            }
            _titleAnimation.PlaySelectAnimation(_options[index]);
            switch (index)
            {
                case 0:
                    // Start Game
                    LoggerManager.Log("Start Game selected");
                    _isGameStarting = true; // 連打制限を有効にする
                    await ScreenEffect.Instance.FadeOut(1f);
                    await Key.Core.AddictiveSceneMananger.UnloadAddictiveScene(Generated.SceneName.Title);
                    await Key.Core.AddictiveSceneMananger.LoadAddictiveScene(Generated.SceneName.InGame);
                    ScreenEffect.Instance.FadeIn(1f);
                    break;
                case 1:
                    // How to Play
                    LoggerManager.Log("How to Play selected");
                    if (!_howToPlayPanelController.IsShowing)
                    {
                        _howToPlayPanelController.ShowHowToPlayPanel();
                    }
                    break;
                case 2:
                    // Settings
                    LoggerManager.Log("Settings selected");
                    if (_settingPanelStateMachine != null && !_settingPanelStateMachine.IsShowing)
                    {
                        _settingPanelStateMachine.Show();
                    }
                    break;
            }
        }
    }
}
