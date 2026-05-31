using UnityEngine;
using DG.Tweening;
using Unity.Netcode;

namespace Ko.Result
{
    public class StageController : NetworkBehaviour
    {
        [SerializeField] private GameObject[] _stages;

        private int GetStageIndexFromPlayerId(int playerId)
        {
            if (_stages == null || _stages.Length == 0) return -1;

            // PlayerPositionSetter と同じ規則で、ClientId をステージ配列インデックスに変換する。
            int length = _stages.Length;
            return ((playerId - 1) % length + length) % length;
        }
        
        // ステージの高さを変更する処理
        // [Rpc(SendTo.Everyone)]
        // public void ChangeStageHeightRpc(int stageNum, float height)
        // {
        //     if (stageNum < 0 || stageNum >= _stages.Length) return;

        //     var stage = _stages[stageNum];
        //     stage.transform.DOScaleY(0.1f, 0.05f).SetEase(Ease.OutQuad)
        //         .OnComplete(() => stage.transform.DOScaleY(height, 1f).SetEase(Ease.OutQuad));
            
        // }

        [Rpc(SendTo.Everyone)]
        public void ChangeStageHeightRpc(int rank, float height, int[] topPlayerIds, float duration)
        {
            int index = GetStageIndexFromPlayerId(topPlayerIds[rank]);
            if (index < 0 || index >= _stages.Length) return;
            var stage = _stages[index];

            stage.transform.DOScaleY(height, duration).SetEase(Ease.OutQuad);
        }
        
    }
}