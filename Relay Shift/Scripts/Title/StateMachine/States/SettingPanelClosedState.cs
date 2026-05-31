using UnityEngine;

namespace Ko
{
    /// <summary>
    /// 設定パネルが閉じている状態
    /// </summary>
    public class SettingPanelClosedState : ISettingPanelState
    {
        private SettingPanelContext _ctx;
        public void Enter(SettingPanelContext context)
        {
            _ctx = context;
            _ctx.CancelCurrentMotion();
            if (_ctx.SettingPanelImage != null)
            {
                _ctx.SettingPanelImage.gameObject.SetActive(false);
                _ctx.SettingPanelImage.rectTransform.localScale = new Vector3(0f, 1f, 1f);
            }
            _ctx.IsShowing = false;
        }

        public void Tick() { }

        public void Exit() { }
    }
}
