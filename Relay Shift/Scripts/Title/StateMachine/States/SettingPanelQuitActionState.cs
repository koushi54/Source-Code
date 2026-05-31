using UnityEngine;

namespace Ko
{
    public class SettingPanelQuitActionState : ISettingPanelState
    {
        private SettingPanelContext _ctx;
        private bool _acted = false;

        public void Enter(SettingPanelContext context)
        {
            _ctx = context;
            _acted = false;
        }

        public void Tick()
        {
            if (!_acted)
            {
                _acted = true;
                Application.Quit();
            }
        }

        public void Exit() { }
    }
}
