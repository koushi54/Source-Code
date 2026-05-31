using Cysharp.Threading.Tasks;

namespace Ko
{
    public interface ISettingPanelState
    {
        void Enter(SettingPanelContext context);
        void Tick();
        void Exit();
    }
}
