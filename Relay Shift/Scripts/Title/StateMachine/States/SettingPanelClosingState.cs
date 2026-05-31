using LitMotion;
using LitMotion.Extensions;
using Cysharp.Threading.Tasks;

namespace Ko
{
    public class SettingPanelClosingState : ISettingPanelState
    {
        private SettingPanelContext _ctx;

        public void Enter(SettingPanelContext context)
        {
            _ctx = context;
            _ctx.CancelCurrentMotion();
            if (_ctx.SettingPanelImage != null)
            {
                _ctx.CurrentMotion = LMotion.Create(1f, 0f, 0.5f)
                    .WithEase(Ease.InBack)
                    .BindToLocalScaleX(_ctx.SettingPanelImage.rectTransform);
            }
            _ctx.IsShowing = false;
        }

        public void Tick()
        {
            if (!_ctx.CurrentMotion.HasValue || !_ctx.CurrentMotion.Value.IsActive())
            {
                var machine = UnityEngine.Object.FindAnyObjectByType<SettingPanelStateMachine>();
                machine?.ChangeState(new SettingPanelClosedState());
            }
        }

        public void Exit() { }
    }
}
