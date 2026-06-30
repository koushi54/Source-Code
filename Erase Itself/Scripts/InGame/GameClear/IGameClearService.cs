using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Ko.InGame.GameClear
{
    public interface IGameClearService
    {
        UniTask PlayGameClearAnimationAsync(float limitTime, float currentTime);
        void GoToNextStage();
    }
}