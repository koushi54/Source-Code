using UnityEngine;

namespace Ko
{
    public class SettingPanelAdjustSliderState : ISettingPanelState
    {
        private SettingPanelContext _ctx;
        private UnityEngine.UI.Slider _registeredSlider;

        public void Enter(SettingPanelContext context)
        {
            _ctx = context;
            _ctx.SetOnlyCurrentSliderInteractable(true);
            ApplyCurrentSliderValueToAudioSettings();
            RegisterSliderListener();
        }

        public void Tick()
        {
            var slider = _ctx.GetCurrentSlider();
            if (slider == null) return;

            if (SettingPanelInput.LeftPressed())
            {
                slider.value = Mathf.Max(slider.value - 1f, slider.minValue);
                ApplyCurrentSliderValueToAudioSettings();
            }
            else if (SettingPanelInput.RightPressed())
            {
                slider.value = Mathf.Min(slider.value + 1f, slider.maxValue);
                ApplyCurrentSliderValueToAudioSettings();
            }
            _ctx.UpdateSliderText();

            if (SettingPanelInput.BackspacePressed())
            {
                _ctx.IsSelectedOption = false;
                _ctx.Animator?.ChangeSliderFillColor(_ctx.SliderFillImages, _ctx.CurrentOptionIndex, _ctx.IsSelectedOption);
                var machine = Object.FindAnyObjectByType<SettingPanelStateMachine>();
                machine?.ChangeState(new SettingPanelNavigateState());
            }
        }

        public void Exit()
        {
            UnregisterSliderListener();
            _ctx.SetOnlyCurrentSliderInteractable(false);
        }

        private void RegisterSliderListener()
        {
            var slider = _ctx.GetCurrentSlider();
            if (slider == null) return;
            // 保持しておき、Exit 時に解除する
            _registeredSlider = slider;
            _registeredSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        private void UnregisterSliderListener()
        {
            if (_registeredSlider == null) return;
            _registeredSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
            _registeredSlider = null;
        }

        private void OnSliderValueChanged(float value)
        {
            // カーソルなどでスライダー値が変化したときに音量を適用
            ApplyCurrentSliderValueToAudioSettings();
            _ctx.UpdateSliderText();
        }

        private void ApplyCurrentSliderValueToAudioSettings()
        {
            var slider = _ctx.GetCurrentSlider();
            if (slider == null)
            {
                return;
            }

            float normalizedValue = Mathf.InverseLerp(slider.minValue, slider.maxValue, slider.value);

            switch (_ctx.CurrentOptionIndex)
            {
                case 0:
                    _ctx.ApplyMasterSliderValueToAudioSettings(normalizedValue);
                    break;
                case 1:
                    _ctx.BgmChannelRelative = normalizedValue;
                    if (_ctx.BgmSettings != null)
                    {
                        _ctx.BgmSettings.SetMasterVolume(Mathf.Clamp01(_ctx.MasterVolume * _ctx.BgmChannelRelative));
                    }
                    break;
                case 2:
                    _ctx.SeChannelRelative = normalizedValue;
                    if (_ctx.SeSettings != null)
                    {
                        _ctx.SeSettings.SetMasterVolume(Mathf.Clamp01(_ctx.MasterVolume * _ctx.SeChannelRelative));
                    }
                    break;
            }
        }
    }
}
