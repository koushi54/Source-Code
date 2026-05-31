using UnityEngine;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine.UI;
using TMPro;
using AudioManager.BGM;
using AudioManager.SE;

namespace Ko
{
    public class SettingPanelStateMachine : MonoBehaviour
    {
        [Header("Context References")]
        [SerializeField] private SettingPanelView _view;
        [SerializeField] private SettingPanelAnimator _animator;
        [SerializeField] private QuitConfirmDialogController _quitConfirmDialog;
        [SerializeField] private BGMSettings _bgmSettings;
        [SerializeField] private SESettings _seSettings;
        [SerializeField] private Image[] _optionBackgrounds;
        [SerializeField] private Slider _master;
        [SerializeField] private Slider _bgm;
        [SerializeField] private Slider _se;
        [SerializeField] private TMP_Text[] _sliderValueTexts;
        [SerializeField] private Image[] _sliderFillImages;

        private SettingPanelContext _context;
        private ISettingPanelState _currentState;

        private void Awake()
        {
            float initialMasterVolume = 1f;
            if (_bgmSettings != null)
            {
                initialMasterVolume = _bgmSettings.MasterVolume;
            }
            else if (_seSettings != null)
            {
                initialMasterVolume = _seSettings.MasterVolume;
            }

            _context = new SettingPanelContext
            {
                SettingPanelImage = _view != null ? _view.PanelImage : null,
                OptionBackgrounds = _optionBackgrounds,
                MasterSlider = _master,
                BgmSlider = _bgm,
                SeSlider = _se,
                SliderValueTexts = _sliderValueTexts,
                SliderFillImages = _sliderFillImages,
                Animator = _animator,
                QuitConfirmDialog = _quitConfirmDialog,
                BgmSettings = _bgmSettings,
                SeSettings = _seSettings,
                MasterVolume = initialMasterVolume
            };
            // Start closed
            ChangeState(new SettingPanelClosedState());
        }

        private void Update()
        {
            _currentState?.Tick();
        }

        public void Show()
        {
            if (_currentState is SettingPanelClosedState)
            {
                ChangeState(new SettingPanelOpeningState());
            }
        }

        public async UniTask Hide()
        {
            // request closing from any state
            ChangeState(new SettingPanelClosingState());
            await UniTask.Yield();
        }

        public bool IsShowing => _context != null && _context.IsShowing;

        internal void ChangeState(ISettingPanelState next)
        {
            _currentState?.Exit();
            _currentState = next;
            _currentState?.Enter(_context);
        }
    }
}
