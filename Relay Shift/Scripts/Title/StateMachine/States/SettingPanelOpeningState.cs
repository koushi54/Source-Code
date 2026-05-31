using LitMotion;
using LitMotion.Extensions;

namespace Ko
{
    public class SettingPanelOpeningState : ISettingPanelState
    {
        private SettingPanelContext _ctx;

        public void Enter(SettingPanelContext context)
        {
            _ctx = context;
            _ctx.CancelCurrentMotion();
            _ctx.SyncAudioSliderValuesFromSettings();
            if (_ctx.SettingPanelImage != null)
            {
                _ctx.SettingPanelImage.gameObject.SetActive(true);
                _ctx.CurrentMotion = LMotion.Create(0f, 1f, 0.5f)
                    .WithEase(Ease.OutBack)
                    .BindToLocalScaleX(_ctx.SettingPanelImage.rectTransform);
            }
            _ctx.IsShowing = true;
        }

        public void Tick()
        {
            if (!_ctx.CurrentMotion.HasValue || !_ctx.CurrentMotion.Value.IsActive())
            {
                // move to navigation state
                var machine = UnityEngine.Object.FindAnyObjectByType<SettingPanelStateMachine>();
                machine?.ChangeState(new SettingPanelNavigateState());
            }
        }

        public void Exit() { }
    }
}
