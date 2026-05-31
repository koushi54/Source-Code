using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitMotion;
using UnityEngine.InputSystem;
using LitMotion.Extensions;
using Cysharp.Threading.Tasks;
using System;

namespace Ko
{
    public class SettingPanelController : MonoBehaviour
    {
        [SerializeField] private Image _settingPanelImage;
        [SerializeField] private Image[] _settingOptionBackgrounds;
        [Header("スライダー関連")]
        [SerializeField] private Slider _masterVolumeSlider;
        [SerializeField] private Slider _bgmVolumeSlider;
        [SerializeField] private Slider _seVolumeSlider;
        [SerializeField] private TMP_Text[] _sliderValueTexts;
        [SerializeField] private Image[] _sliderFillImages;

        [SerializeField] private SettingPanelAnimator _settingPanelAnimator;
        private MotionHandle _panelMotion;
        private int _currentOptionIndex = 0;
        public bool IsShowing = false;
        private bool _isSelectedOption = false;


        private enum SettingOption
        {
            Master,
            BGM,
            SE,
            Quit
        }

        private void Start()
        {
            _settingPanelImage.gameObject.SetActive(false);
            _settingPanelImage.rectTransform.localScale = new Vector3(0f, 1f, 1f); // 初期状態は横に縮んでいる
        }

        private void Update()
        {
            if (IsShowing && Keyboard.current.backspaceKey.wasPressedThisFrame)
            {
                HideSettingPanel().Forget();
            }
            if (IsShowing)
            {
                SelectSettingOption();
            }
        }

        public void ShowSettingPanel()
        {
            IsShowing = true;
            _settingPanelImage.gameObject.SetActive(true);

            _panelMotion = LMotion.Create(0f, 1f, 0.5f)
                .WithEase(Ease.OutBack)
                .BindToLocalScaleX(_settingPanelImage.rectTransform);
        }

        public async UniTask HideSettingPanel()
        {
            _panelMotion = LMotion.Create(1f, 0f, 0.5f)
                .WithEase(Ease.InBack)
                .BindToLocalScaleX(_settingPanelImage.rectTransform);
            await _panelMotion.ToAwaitable();
            _settingPanelImage.gameObject.SetActive(false);
            IsShowing = false;
        }

        private void SelectSettingOption()
        {
            if (Keyboard.current.upArrowKey.wasPressedThisFrame && !_isSelectedOption)
            {
                _currentOptionIndex = (_currentOptionIndex - 1 + _settingOptionBackgrounds.Length) % _settingOptionBackgrounds.Length;
                _settingPanelAnimator.ChangeOptionBackground(_settingOptionBackgrounds, _currentOptionIndex);
            }
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame && !_isSelectedOption)
            {
                _currentOptionIndex = (_currentOptionIndex + 1) % _settingOptionBackgrounds.Length;
                _settingPanelAnimator.ChangeOptionBackground(_settingOptionBackgrounds, _currentOptionIndex);
            }
            else if (Keyboard.current.enterKey.wasPressedThisFrame && !_isSelectedOption)
            {
                _isSelectedOption = true;
                _settingPanelAnimator.ChangeSliderFillColor(_sliderFillImages, _currentOptionIndex, _isSelectedOption);
                switch ((SettingOption)_currentOptionIndex)
                {
                    case SettingOption.Master:
                        LoggerManager.Log("Master volume selected");
                        ControlSlider();
                        break;
                    case SettingOption.BGM:
                        LoggerManager.Log("BGM volume selected");
                        ControlSlider();
                        break;
                    case SettingOption.SE:
                        LoggerManager.Log("SE volume selected");
                        ControlSlider();
                        break;
                    case SettingOption.Quit:
                        LoggerManager.Log("Quit selected");
                        Application.Quit();
                        break;
                }
            }
        }

        private void ControlSlider()
        {
            // 現在選択されているオプションに応じて、スライダーの値を変更する処理をここに実装
            // 例えば、Master音量が選択されている場合は、Master音量のスライダーの値を変更するなど
            switch ((SettingOption)_currentOptionIndex)
            {
                case SettingOption.Master:
                    // Master音量のスライダーの値を変更
                    if (Keyboard.current.leftArrowKey.wasPressedThisFrame && _masterVolumeSlider.value > 0f)
                    {
                        _masterVolumeSlider.value = Mathf.Max(_masterVolumeSlider.value - 1f, 0f);
                    }
                    else if (Keyboard.current.rightArrowKey.isPressed && _masterVolumeSlider.value < 10f)
                    {
                        _masterVolumeSlider.value = Mathf.Min(_masterVolumeSlider.value + 1f, 10f);
                    }
                    _sliderValueTexts[0].text = _masterVolumeSlider.value.ToString("F2");
                    break;
                case SettingOption.BGM:
                    // BGM音量のスライダーの値を変更
                    if (Keyboard.current.leftArrowKey.wasPressedThisFrame && _bgmVolumeSlider.value > 0f)
                    {
                        _bgmVolumeSlider.value = Mathf.Max(_bgmVolumeSlider.value - 1f, 0f);
                    }
                    else if (Keyboard.current.rightArrowKey.isPressed && _bgmVolumeSlider.value < 10f)
                    {
                        _bgmVolumeSlider.value = Mathf.Min(_bgmVolumeSlider.value + 1f, 10f);
                    }
                    _sliderValueTexts[1].text = _bgmVolumeSlider.value.ToString("F2");
                    break;
                case SettingOption.SE:
                    // SE音量のスライダーの値を変更
                    if (Keyboard.current.leftArrowKey.wasPressedThisFrame && _seVolumeSlider.value > 0f)
                    {
                        _seVolumeSlider.value = Mathf.Max(_seVolumeSlider.value - 1f, 0f);
                    }
                    else if (Keyboard.current.rightArrowKey.isPressed && _seVolumeSlider.value < 10f)
                    {
                        _seVolumeSlider.value = Mathf.Min(_seVolumeSlider.value + 1f, 10f);
                    }
                    _sliderValueTexts[2].text = _seVolumeSlider.value.ToString("F2");
                    break;
            }
            if (Keyboard.current.backspaceKey.wasPressedThisFrame && _isSelectedOption)
            {
                _isSelectedOption = false;
                _settingPanelAnimator.ChangeSliderFillColor(_sliderFillImages, _currentOptionIndex, _isSelectedOption);
            }
        }
    }
}