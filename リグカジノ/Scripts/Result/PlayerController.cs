using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

namespace Ko.Result
{
    public class PlayerController : NetworkBehaviour
    {
        private List<int> _topPlayerIds = new List<int>(); // 上位プレイヤーのIDを保持するリスト
        [SerializeField] private GameObject _celebrationEffectPrefab; // お祝いエフェクトのプレハブ

        // 所持金の多いプレイヤー上位3人分のIDを取得
        public int[] GetTopPlayerIds(List<int> currentMoneyList, ulong[] playerIds)
        {
            int targetCount = Mathf.Min(currentMoneyList.Count, playerIds.Length);
            List<(int playerId, int money)> sortedPlayers = new List<(int playerId, int money)>(targetCount);

            for (int i = 0; i < targetCount; i++)
            {
                sortedPlayers.Add(((int)playerIds[i], currentMoneyList[i]));
            }

            sortedPlayers.Sort((a, b) => b.money.CompareTo(a.money));

            _topPlayerIds.Clear();
            for (int i = 0; i < Mathf.Min(3, sortedPlayers.Count); i++)
            {
                _topPlayerIds.Add(sortedPlayers[i].playerId);
            }

            return _topPlayerIds.ToArray();
        }

        /// 戻り値を NetworkObjectReference[] に変更
        public NetworkObjectReference[] GetTopPlayerObjects(NetworkObject[] playerObjects, int[] topPlayerIds)
        {
            NetworkObjectReference[] topPlayerObjects = new NetworkObjectReference[topPlayerIds.Length];
            for (int i = 0; i < topPlayerIds.Length; i++)
            {
                int playerId = topPlayerIds[i];
                NetworkObject playerObject = playerObjects.FirstOrDefault(obj => obj != null && obj.OwnerClientId == (ulong)playerId);

                if (playerObject != null)
                {
                    topPlayerObjects[i] = playerObject;
                }
            }
            return topPlayerObjects;
        }

        // 引数の型を NetworkObjectReference[] に変更
        [Rpc(SendTo.Everyone)]
        public void MovingPlayerToTopPositionRpc(int rank, float heightDelta, NetworkObjectReference[] topPlayerObjects, float duration)
        {
            // 配列の範囲チェック
            if (rank < 0 || rank >= topPlayerObjects.Length) return;

            // NetworkObjectReference から実際の NetworkObject を取得
            if (topPlayerObjects[rank].TryGet(out NetworkObject playerObj))
            {
                Vector3 currentPosition = playerObj.transform.position;
                Vector3 targetPosition = new Vector3(currentPosition.x, currentPosition.y + heightDelta, currentPosition.z);
                playerObj.transform.DOMove(targetPosition, duration).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    GameObject effect = Instantiate(_celebrationEffectPrefab, targetPosition + Vector3.up * 1f, Quaternion.identity);
                });
            }
        }
    }
}
