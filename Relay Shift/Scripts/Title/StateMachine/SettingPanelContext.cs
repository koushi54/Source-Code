using UnityEngine.UI;
using UnityEngine;
using System;
using LitMotion;
using Cysharp.Threading.Tasks;
using AudioManager.BGM;
using AudioManager.SE;

namespace Ko
{
    public class SettingPanelContext
    {
        public Image SettingPanelImage;
        public Image[] OptionBackgrounds;
        public Slider MasterSlider;
        public Slider BgmSlider;
        public Slider SeSlider;
        public TMPro.TMP_Text[] SliderValueTexts;
        public Image[] SliderFillImages;
        public QuitConfirmDialogController QuitConfirmDialog;
        public SettingPanelAnimator Animator;
        public BGMSettings BgmSettings;
        public SESettings SeSettings;
        public float MasterVolume = 1f;
        public float BgmChannelRelative = 1f;
        public float SeChannelRelative = 1f;

        public int CurrentOptionIndex = 0;
        public bool IsSelectedOption = false;
        public bool IsShowing = false;
        public MotionHandle? CurrentMotion;

        public void CancelCurrentMotion()
        {
            if (CurrentMotion.HasValue && CurrentMotion.Value.IsActive())
            {
                var m = CurrentMotion.Value;
                m.Cancel();
            }
            CurrentMotion = null;
        }

        public Slider GetCurrentSlider()
        {
            switch (CurrentOptionIndex)
            {
                case 0: return MasterSlider;
                case 1: return BgmSlider;
                case 2: return SeSlider;
                default: return null;
            }
        }

        public void UpdateSliderText()
        {
            if (SliderValueTexts == null) return;
            // スライダーの値を整数で表示
            if (MasterSlider != null && SliderValueTexts.Length > 0) SliderValueTexts[0].text = MasterSlider.value.ToString("F0");
            if (BgmSlider != null && SliderValueTexts.Length > 1) SliderValueTexts[1].text = BgmSlider.value.ToString("F0");
            if (SeSlider != null && SliderValueTexts.Length > 2) SliderValueTexts[2].text = SeSlider.value.ToString("F0");
        }

        public float GetMasterSliderValueFromSettings()
        {
            return MasterVolume;
        }

        public void ApplyMasterSliderValueToAudioSettings(float normalizedValue)
        {
            MasterVolume = normalizedValue;
            // Apply master multiplied by per-channel relative factors so master always caps channel volumes
            if (BgmSettings != null)
            {
                BgmSettings.SetMasterVolume(Mathf.Clamp01(MasterVolume * BgmChannelRelative));
            }

            if (SeSettings != null)
            {
                SeSettings.SetMasterVolume(Mathf.Clamp01(MasterVolume * SeChannelRelative));
            }
        }

        public void SyncAudioSliderValuesFromSettings()
        {
            // Derive master and per-channel relative values from existing settings.
            float bgmMaster = BgmSettings != null ? BgmSettings.MasterVolume : 0f;
            float seMaster = SeSettings != null ? SeSettings.MasterVolume : 0f;

            // Choose master as the maximum of existing channel masters to keep channelRelative <= 1
            float derivedMaster = Mathf.Max(bgmMaster, seMaster, 0.0001f);
            MasterVolume = derivedMaster;

            BgmChannelRelative = bgmMaster > 0f ? Mathf.Clamp01(bgmMaster / MasterVolume) : 1f;
            SeChannelRelative = seMaster > 0f ? Mathf.Clamp01(seMaster / MasterVolume) : 1f;

            if (MasterSlider != null)
            {
                MasterSlider.value = Mathf.Lerp(MasterSlider.minValue, MasterSlider.maxValue, MasterVolume);
            }

            if (BgmSlider != null)
            {
                BgmSlider.value = Mathf.Lerp(BgmSlider.minValue, BgmSlider.maxValue, BgmChannelRelative);
            }

            if (SeSlider != null)
            {
                SeSlider.value = Mathf.Lerp(SeSlider.minValue, SeSlider.maxValue, SeChannelRelative);
            }

            UpdateSliderText();
        }

        public void SetOnlyCurrentSliderInteractable(bool isInteractable)
        {
            if (MasterSlider != null)
            {
                MasterSlider.interactable = isInteractable && CurrentOptionIndex == 0;
            }

            if (BgmSlider != null)
            {
                BgmSlider.interactable = isInteractable && CurrentOptionIndex == 1;
            }

            if (SeSlider != null)
            {
                SeSlider.interactable = isInteractable && CurrentOptionIndex == 2;
            }
        }
    }
}
