using UnityEngine;

namespace Ko
{
    public class SettingPanelNavigateState : ISettingPanelState
    {
        private SettingPanelContext _ctx;

        public void Enter(SettingPanelContext context)
        {
            _ctx = context;
            // ensure visuals
            _ctx.Animator?.ChangeOptionBackground(_ctx.OptionBackgrounds, _ctx.CurrentOptionIndex);
        }

        public void Tick()
        {
            if (SettingPanelInput.UpPressed())
            {
                _ctx.CurrentOptionIndex = (_ctx.CurrentOptionIndex - 1 + _ctx.OptionBackgrounds.Length) % _ctx.OptionBackgrounds.Length;
                _ctx.Animator?.ChangeOptionBackground(_ctx.OptionBackgrounds, _ctx.CurrentOptionIndex);
            }
            else if (SettingPanelInput.DownPressed())
            {
                _ctx.CurrentOptionIndex = (_ctx.CurrentOptionIndex + 1) % _ctx.OptionBackgrounds.Length;
                _ctx.Animator?.ChangeOptionBackground(_ctx.OptionBackgrounds, _ctx.CurrentOptionIndex);
            }
            else if (SettingPanelInput.EnterPressed())
            {
                _ctx.IsSelectedOption = true;
                _ctx.Animator?.ChangeSliderFillColor(_ctx.SliderFillImages, _ctx.CurrentOptionIndex, _ctx.IsSelectedOption);
                // branch
                if (_ctx.CurrentOptionIndex == 3)
                {
                    var machine = Object.FindAnyObjectByType<SettingPanelStateMachine>();
                    machine?.ChangeState(new SettingPanelQuitConfirmState());
                }
                else
                {
                    var machine = Object.FindAnyObjectByType<SettingPanelStateMachine>();
                    machine?.ChangeState(new SettingPanelAdjustSliderState());
                }
            }
            else if (SettingPanelInput.BackspacePressed())
            {
                var machine = Object.FindAnyObjectByType<SettingPanelStateMachine>();
                machine?.ChangeState(new SettingPanelClosingState());
            }
        }

        public void Exit() { }
    }
}
