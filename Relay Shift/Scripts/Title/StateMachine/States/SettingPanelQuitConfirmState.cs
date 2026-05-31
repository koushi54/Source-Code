using Ko;
using UnityEngine;

namespace Ko
{
    public class SettingPanelQuitConfirmState : ISettingPanelState
    {
        private SettingPanelContext _ctx;

        public void Enter(SettingPanelContext context)
        {
            _ctx = context;
            var quitConfirmDialog = _ctx.QuitConfirmDialog;
            quitConfirmDialog?.Show();
        }

        public void Tick()
        {
            // 左右キーで「はい」「いいえ」を選択できるようにする
            if (SettingPanelInput.LeftPressed())
            {
                _ctx.QuitConfirmDialog?.SetSelectedOption(true); // 「はい」を選択
            }
            else if (SettingPanelInput.RightPressed())
            {
                _ctx.QuitConfirmDialog?.SetSelectedOption(false); // 「いいえ」を選択
            }
            else if (SettingPanelInput.EnterPressed())
            {
                // 選択されているオプションに応じて処理を分岐
                if (_ctx.QuitConfirmDialog != null)
                {
                    if (_ctx.QuitConfirmDialog.IsYesSelected)
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#else
                        Application.Quit(); // ゲームを終了
#endif
                    }
                    else
                    {
                        var machine = Object.FindAnyObjectByType<SettingPanelStateMachine>();
                        machine?.ChangeState(new SettingPanelNavigateState()); // 設定パネルのナビゲート状態に戻る
                    }
                }
            }
            else if (SettingPanelInput.BackspacePressed())
            {
                var machine = Object.FindAnyObjectByType<SettingPanelStateMachine>();
                machine?.ChangeState(new SettingPanelNavigateState()); // 設定パネルのナビゲート状態に戻る
            }
        }


        public void Exit()
        {
            var quitConfirmDialog = Object.FindAnyObjectByType<QuitConfirmDialogController>();
            quitConfirmDialog?.Hide();
        }
    }
}