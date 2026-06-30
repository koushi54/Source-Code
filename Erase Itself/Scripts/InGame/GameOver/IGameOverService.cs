using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Ko.InGame.GameOver
{
    public interface IGameOverService
    {
        UniTask PlayGameOverAnimationAsync();
        void ContinueGame();
    }
}